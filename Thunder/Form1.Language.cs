using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Thunder
{
    public partial class mainWindow
    {
        // 介面語言：中文 / 英文-----------------------------------------------------------------------------------------
        // 翻譯固定的介面文字（選單、按鈕、標籤、分頁、欄名、下拉選項、提示）；狀態列與訊息視窗仍為中文
        private const string LanguageKey = @"HKEY_CURRENT_USER\Software\PatternMagic";
        public static bool English { get; private set; } = ReadLanguage();
        private static bool ReadLanguage()
        {
            try
            {
                return (Registry.GetValue(LanguageKey, "Language", "zh") as string) == "en";
            }
            catch (Exception)
            {
                return false;
            }
        }  // 讀取上次的語言
        public static string T(string text)
        {
            // 英文介面時翻譯；整句查不到時逐行翻譯，仍查不到的行保留中文
            if (!English || string.IsNullOrEmpty(text) || !HasChinese(text)) return text;
            if (TranslateLine(text, out string english)) return english;
            if (!text.Contains("\n")) return text;
            return string.Join("\r\n", text.Replace("\r\n", "\n").Split('\n').Select(line => TranslateLine(line, out string l) ? l : line));
        }  // 翻譯
        private static bool TranslateLine(string line, out string english)
        {
            // 先查對照表，再試依名稱產生的句型（例如「Mask 範本 dot1：…」）
            if (LanguageTable.TryGetValue(line, out english)) return true;
            foreach (var (pattern, replacement) in LanguagePatterns)
            {
                var match = pattern.Match(line);
                if (match.Success) { english = match.Result(replacement); return true; }
            }
            english = line;
            return false;
        }  // 翻譯一行
        private static readonly (System.Text.RegularExpressions.Regex pattern, string replacement)[] LanguagePatterns =
        {
            (new System.Text.RegularExpressions.Regex(@"^Mask 範本 (\w+)：亮點用「選擇顏色」（子像素圖樣用其亮度），暗點為黑$"), "Mask template $1: lit cells use the selected color (sub-pixel patterns use its brightness), dark cells are black"),
            (new System.Text.RegularExpressions.Regex(@"^(\w+)：按下後排進畫面，再按一次取消$"), "$1: press to add it to the screen, press again to remove"),
            (new System.Text.RegularExpressions.Regex(@"^子像素亮度設為 (\d+)$"), "Set sub-pixel brightness to $1"),
            (new System.Text.RegularExpressions.Regex(@"^蒙太奇亮度設為 (\d+)$"), "Set montage brightness to $1"),
            (new System.Text.RegularExpressions.Regex(@"^右斜(\d)$"), "R-diag $1"),
            (new System.Text.RegularExpressions.Regex(@"^左斜(\d)$"), "L-diag $1"),
            (new System.Text.RegularExpressions.Regex(@"^每 ?([\d.]+) ?秒$"), "Every $1 s"),
            (new System.Text.RegularExpressions.Regex(@"^每次 \+(\d+) 階$"), "+$1 levels each step"),
            (new System.Text.RegularExpressions.Regex(@"^水平 (\d+) 格、垂直 (\d+) 格$"), "$1 horizontal x $2 vertical cells"),
            (new System.Text.RegularExpressions.Regex(@"^(\d+) 倍$"), "$1x"),
            (new System.Text.RegularExpressions.Regex(@"^旋轉(\d+)°?$"), "Rotate $1°"),
            (new System.Text.RegularExpressions.Regex(@"^程式資料夾：(.*)$"), "Program folder: $1"),
            (new System.Text.RegularExpressions.Regex(@"^還原（可還原 (\d+) 步）$"), "Undo ($1 steps available)"),
            (new System.Text.RegularExpressions.Regex(@"^復原（可復原 (\d+) 步）$"), "Undo ($1 steps available)"),
            (new System.Text.RegularExpressions.Regex(@"^(\w+)：滑鼠移到參數、選項欄位可看寫法$"), "$1: hover over Parameter / Option to see the syntax"),
        };  // 依名稱產生的句型
        private static bool HasChinese(string text) => text.Any(c => c >= 0x3000 && c <= 0x9FFF || c >= 0xFF00 && c <= 0xFFEF);
        // 每個元件原本的中文文字（切回中文時還原）；程式中途改過文字時以新的文字為準
        private sealed class TextRecord { public string Original, Shown; }
        private static readonly ConditionalWeakTable<object, Dictionary<string, TextRecord>> Originals = new ConditionalWeakTable<object, Dictionary<string, TextRecord>>();
        private static string Localized(object owner, string slot, string current)
        {
            var records = Originals.GetOrCreateValue(owner);
            if (!records.TryGetValue(slot, out TextRecord record) || record.Shown != current)
            {
                record = new TextRecord { Original = current };  // 第一次，或程式改過文字
            }
            record.Shown = English ? T(record.Original) : record.Original;
            records[slot] = record;
            return record.Shown;
        }  // 依目前語言取得要顯示的文字
        public static void Localize(Control root)
        {
            // 套用目前語言到整個控制項樹（含右鍵選單、欄名、下拉選項、提示）
            if (root == null) return;
            if (!English && !Originals.TryGetValue(root, out _) && !HasRecords(root)) return;  // 從沒翻譯過、又是中文：不用處理
            var menus = new HashSet<ToolStrip>();
            void Walk(Control control)
            {
                if (!(control is TextBoxBase) && !(control is NumericUpDown) && !(control is ComboBox) && !(control is DateTimePicker) && control.Text != "")
                {
                    string text = Localized(control, "Text", control.Text);
                    if (control.Text != text) control.Text = text;
                }
                switch (control)
                {
                    case ComboBox combo when combo.DropDownStyle == ComboBoxStyle.DropDownList:
                        LocalizeComboItems(combo);
                        break;
                    case ListView list:
                        foreach (ColumnHeader column in list.Columns)
                        {
                            string text = Localized(column, "Text", column.Text);
                            if (column.Text != text) column.Text = text;
                        }
                        break;
                    case ToolStrip strip:
                        menus.Add(strip);
                        break;
                }
                if (control.ContextMenuStrip != null) menus.Add(control.ContextMenuStrip);
                foreach (Control child in control.Controls) Walk(child);
            }
            Walk(root);
            // 表單欄位中的右鍵選單（沒有指定給控制項的也算）
            foreach (FieldInfo field in root.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (typeof(ContextMenuStrip).IsAssignableFrom(field.FieldType) && field.GetValue(root) is ContextMenuStrip menu) menus.Add(menu);
                if (typeof(ToolTip).IsAssignableFrom(field.FieldType) && field.GetValue(root) is ToolTip tip) LocalizeToolTip(tip);
            }
            foreach (ToolStrip strip in menus) LocalizeItems(strip.Items);
        }  // 套用語言
        private static bool HasRecords(Control root) => root.Controls.Cast<Control>().Any(c => Originals.TryGetValue(c, out _));
        private static void LocalizeItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                if (!(item is ToolStripTextBox) && !(item is ToolStripComboBox) && item.Text != "")
                {
                    string text = Localized(item, "Text", item.Text);
                    if (item.Text != text) item.Text = text;
                }
                if (!string.IsNullOrEmpty(item.ToolTipText))
                {
                    string tip = Localized(item, "Tip", item.ToolTipText);
                    if (item.ToolTipText != tip) item.ToolTipText = tip;
                }
                if (item is ToolStripComboBox combo && combo.DropDownStyle == ComboBoxStyle.DropDownList) LocalizeComboItems(combo.ComboBox);
                if (item is ToolStripDropDownItem dropDown && dropDown.HasDropDownItems) LocalizeItems(dropDown.DropDownItems);
            }
        }  // 選單項目
        private static void LocalizeComboItems(ComboBox combo)
        {
            // 只翻譯文字項目，保留選取的索引（程式都用索引判斷）
            int selected = combo.SelectedIndex;
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (!(combo.Items[i] is string item)) continue;
                string text = Localized(combo, $"Item{i}", item);
                if (item != text) combo.Items[i] = text;
            }
            if (combo.SelectedIndex != selected && selected < combo.Items.Count) combo.SelectedIndex = selected;
        }  // 下拉選項
        private static readonly FieldInfo ToolTipTools = typeof(ToolTip).GetField("tools", BindingFlags.Instance | BindingFlags.NonPublic);
        private static void LocalizeToolTip(ToolTip tip)
        {
            // ToolTip 沒有公開列舉的方法：從內部的 tools 表取得所有設定過提示的控制項
            if (!(ToolTipTools?.GetValue(tip) is Hashtable tools)) return;
            foreach (Control control in tools.Keys.OfType<Control>().ToList())
            {
                string current = tip.GetToolTip(control);
                if (string.IsNullOrEmpty(current)) continue;
                string text = Localized(control, "Tip:" + tip.GetHashCode(), current);
                if (current != text) tip.SetToolTip(control, text);
            }
        }  // 提示文字
        private ToolStripMenuItem mnsLanguage_btn;
        private void InitLanguage()
        {
            // About 右邊的語言切換：顯示要切換過去的語言
            mnsLanguage_btn = new ToolStripMenuItem(English ? "中文" : "English") { ToolTipText = English ? "切換成中文介面" : "Switch to English interface" };
            mnsLanguage_btn.Click += (s, e) => SetLanguage(!English);
            menuStrip.Items.Insert(menuStrip.Items.IndexOf(mnsAbout_btn) + 1, mnsLanguage_btn);
            if (English) LocalizeAllForms();
        }  // 建立語言切換
        private void SetLanguage(bool english)
        {
            English = english;
            try
            {
                Registry.SetValue(LanguageKey, "Language", english ? "en" : "zh");
            }
            catch (Exception)
            {
                // 無法寫入登錄：只影響下次啟動
            }
            LocalizeAllForms();
            UpdateConfigHelp();  // 編輯區提示依類型重設
            UpdateRefreshRate();
            ssrStatus_lbl.Text = english ? "Interface: English (status messages and dialogs remain in Chinese)" : "介面：中文";
        }  // 切換語言
        private void LocalizeAllForms()
        {
            foreach (Form form in Application.OpenForms.Cast<Form>().ToList()) Localize(form);
            mnsLanguage_btn.Text = English ? "中文" : "English";
            mnsLanguage_btn.ToolTipText = English ? "切換成中文介面" : "Switch to English interface";
        }  // 所有開著的視窗套用語言
    }
}
