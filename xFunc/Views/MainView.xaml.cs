﻿// Copyright 2012-2013 Dmitry Kischenko
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either 
// express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using xFunc.Logics;
using xFunc.Logics.Expressions;
using xFunc.Maths;
using xFunc.Maths.Expressions;
using xFunc.Presenters;
using xFunc.Properties;
using xFunc.Resources;
using xFunc.ViewModels;

namespace xFunc.Views
{

    public partial class MainView : Fluent.RibbonWindow
    {

        private MathProcessor processor;

        private MathPresenter mathPresenter;
        private LogicPresenter logicPresenter;
        private GraphsPresenter graphsPresenter;
        private TruthTablePresenter truthTablePresenter;
        private Updater updater;

        #region Commands

        public static RoutedCommand DegreeCommand = new RoutedCommand();
        public static RoutedCommand RadianCommand = new RoutedCommand();
        public static RoutedCommand GradianCommand = new RoutedCommand();

        public static RoutedCommand BinCommand = new RoutedCommand();
        public static RoutedCommand OctCommand = new RoutedCommand();
        public static RoutedCommand DecCommand = new RoutedCommand();
        public static RoutedCommand HexCommand = new RoutedCommand();

        public static RoutedCommand VariablesCommand = new RoutedCommand();
        public static RoutedCommand FunctionsCommand = new RoutedCommand();

        public static RoutedCommand DeleteExpCommand = new RoutedCommand();
        public static RoutedCommand ClearCommand = new RoutedCommand();

        public static RoutedCommand AboutCommand = new RoutedCommand();

        #endregion Commands

        private VariableView variableView;
        private FunctionView functionView;

        public MainView()
        {
            InitializeComponent();

            processor = new MathProcessor();

            mathPresenter = new MathPresenter(this.mathControl, processor);
            this.mathControl.Presenter = mathPresenter;
            logicPresenter = new LogicPresenter(this.logicControl);
            this.logicControl.Presenter = logicPresenter;
            graphsPresenter = new GraphsPresenter(this.graphsControl, processor);
            this.graphsControl.Presenter = graphsPresenter;
            truthTablePresenter = new TruthTablePresenter();
            this.truthTableControl.Presenter = truthTablePresenter;
            updater = new Updater();

            LoadSettings();

            SetFocus();

            switch (mathPresenter.AngleMeasurement)
            {
                case AngleMeasurement.Degree:
                    degreeButton.IsChecked = true;
                    break;
                case AngleMeasurement.Radian:
                    radianButton.IsChecked = true;
                    break;
                case AngleMeasurement.Gradian:
                    gradianButton.IsChecked = true;
                    break;
            }
            switch (mathPresenter.Base)
            {
                case NumeralSystem.Binary:
                    binButton.IsChecked = true;
                    break;
                case NumeralSystem.Octal:
                    octButton.IsChecked = true;
                    break;
                case NumeralSystem.Decimal:
                    decButton.IsChecked = true;
                    break;
                case NumeralSystem.Hexidecimal:
                    hexButton.IsChecked = true;
                    break;
            }
        }

        private void this_Loaded(object o, RoutedEventArgs args)
        {
            if (Settings.Default.CheckUpdates)
            {
                var updaterTask = Task.Factory.StartNew(() => updater.CheckUpdates());
                updaterTask.ContinueWith(t =>
                {
                    if (t.Result)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.updateText.Text = Resource.AvailableUpdate;
                            this.statusUpdate.Visibility = Visibility.Visible;
                        }));
                    }
                });
            }
        }

        private void UpdateText_MouseUp(object o, MouseButtonEventArgs args)
        {
            if (updater.HasUpdates)
            {
                Process.Start(updater.UpdateUrl);
            }
        }

        private void hideNotification_Click(object o, RoutedEventArgs args)
        {
            this.updateText.Text = string.Empty;
            this.statusUpdate.Visibility = Visibility.Collapsed;
        }

        private void dontCheckUpdates_Click(object o, RoutedEventArgs args)
        {
            hideNotification_Click(o, args);

            Settings.Default.CheckUpdates = false;
            Settings.Default.Save();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            SaveSettings();

            base.OnClosing(e);
        }

        private void LoadSettings()
        {
            if (Settings.Default.WindowState != WindowState.Minimized)
            {
                WindowState = Settings.Default.WindowState;

                if (Settings.Default.WindowTop == 0 || Settings.Default.WindowLeft == 0)
                {
                    Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
                    Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
                }
                else
                {
                    Top = Settings.Default.WindowTop;
                    Left = Settings.Default.WindowLeft;
                }
            }
            Width = Settings.Default.WindowWidth;
            Height = Settings.Default.WindowHeight;

            mathPresenter.AngleMeasurement = Settings.Default.AngleMeasurement;
            mathPresenter.Base = Settings.Default.NumberBase;

            tabControl.SelectedIndex = Settings.Default.SelectedTabIndex;

            numberToolBar.IsExpanded = Settings.Default.NumbersExpanded;
            standartMathToolBar.IsExpanded = Settings.Default.StandartMathExpanded;
            trigonometricToolBar.IsExpanded = Settings.Default.TrigonometricExpanded;
            hyperbolicToolBar.IsExpanded = Settings.Default.HyperbolicExpanded;
            bitwiseToolBar.IsExpanded = Settings.Default.BitwiseExpanded;
            constantsMathToolBar.IsExpanded = Settings.Default.ConstantsMathExpanded;
            additionalMathToolBar.IsExpanded = Settings.Default.AdditionalMathExpanded;

            standartLogicToolBar.IsExpanded = Settings.Default.StandartLogicExpanded;
            constantsLogicToolBar.IsExpanded = Settings.Default.ConstantsLogicExpanded;
            additionalLogicToolBar.IsExpanded = Settings.Default.AdditionalLogicExpanded;
        }

        private void SaveSettings()
        {
            if (WindowState != WindowState.Minimized)
                Settings.Default.WindowState = WindowState;

            Settings.Default.WindowTop = Top;
            Settings.Default.WindowLeft = Left;

            Settings.Default.WindowWidth = Width;
            Settings.Default.WindowHeight = Height;

            Settings.Default.AngleMeasurement = mathPresenter.AngleMeasurement;
            Settings.Default.NumberBase = mathPresenter.Base;

            Settings.Default.SelectedTabIndex = tabControl.SelectedIndex;

            Settings.Default.NumbersExpanded = numberToolBar.IsExpanded;
            Settings.Default.StandartMathExpanded = standartMathToolBar.IsExpanded;
            Settings.Default.TrigonometricExpanded = trigonometricToolBar.IsExpanded;
            Settings.Default.HyperbolicExpanded = hyperbolicToolBar.IsExpanded;
            Settings.Default.BitwiseExpanded = bitwiseToolBar.IsExpanded;
            Settings.Default.ConstantsMathExpanded = constantsMathToolBar.IsExpanded;
            Settings.Default.AdditionalMathExpanded = additionalMathToolBar.IsExpanded;

            Settings.Default.StandartLogicExpanded = standartLogicToolBar.IsExpanded;
            Settings.Default.ConstantsLogicExpanded = constantsLogicToolBar.IsExpanded;
            Settings.Default.AdditionalLogicExpanded = additionalLogicToolBar.IsExpanded;

            Settings.Default.Save();
        }

        private void SetFocus()
        {
            if (tabControl.SelectedItem == mathTab)
                this.mathControl.mathExpressionBox.Focus();
            if (tabControl.SelectedItem == logicTab)
                this.logicControl.logicExpressionBox.Focus();
            if (tabControl.SelectedItem == graphsTab)
                this.graphsControl.graphExpressionBox.Focus();
            if (tabControl.SelectedItem == truthTableTab)
                this.truthTableControl.truthTableExpressionBox.Focus();
        }

        #region Commands

        private void DergeeButton_Execute(object o, ExecutedRoutedEventArgs args)
        {
            radianButton.IsChecked = false;
            gradianButton.IsChecked = false;
            mathPresenter.AngleMeasurement = AngleMeasurement.Degree;
            degreeButton.IsChecked = true;
        }

        private void RadianButton_Execute(object o, ExecutedRoutedEventArgs args)
        {
            degreeButton.IsChecked = false;
            gradianButton.IsChecked = false;
            mathPresenter.AngleMeasurement = AngleMeasurement.Radian;
            radianButton.IsChecked = true;
        }

        private void GradianButton_Execute(object o, ExecutedRoutedEventArgs args)
        {
            degreeButton.IsChecked = false;
            radianButton.IsChecked = false;
            mathPresenter.AngleMeasurement = AngleMeasurement.Gradian;
            gradianButton.IsChecked = true;
        }

        private void AndleButtons_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = tabControl.SelectedItem == mathTab;
        }

        private void BinCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            octButton.IsChecked = false;
            decButton.IsChecked = false;
            hexButton.IsChecked = false;
            mathPresenter.Base = NumeralSystem.Binary;
            binButton.IsChecked = true;
        }

        private void OctCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            binButton.IsChecked = false;
            decButton.IsChecked = false;
            hexButton.IsChecked = false;
            mathPresenter.Base = NumeralSystem.Octal;
            octButton.IsChecked = true;
        }

        private void DecCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            binButton.IsChecked = false;
            octButton.IsChecked = false;
            hexButton.IsChecked = false;
            mathPresenter.Base = NumeralSystem.Decimal;
            decButton.IsChecked = true;
        }

        private void HexCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            binButton.IsChecked = false;
            octButton.IsChecked = false;
            decButton.IsChecked = false;
            mathPresenter.Base = NumeralSystem.Hexidecimal;
            hexButton.IsChecked = true;
        }

        private void BaseCommands_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = tabControl.SelectedItem == mathTab;
        }

        private void VariablesCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            if (variableView == null)
            {
                variableView = new VariableView(this.processor) { Owner = this };
                variableView.Closed += (lo, larg) => variableView = null;
            }

            if (variableView.Visibility == Visibility.Visible)
                variableView.Activate();
            else
                variableView.Visibility = Visibility.Visible;
        }

        private void VariablesCommand_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = tabControl.SelectedItem == mathTab;
        }

        private void FunctionsCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            if (functionView == null)
            {
                functionView = new FunctionView(processor) { Owner = this };
                functionView.Closed += (lo, larg) => functionView = null;
            }

            if (functionView.Visibility == Visibility.Visible)
                functionView.Activate();
            else
                functionView.Visibility = Visibility.Visible;
        }

        private void FunctionsCommand_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = tabControl.SelectedItem == mathTab;
        }

        private void DeleteExp_Execute(object o, ExecutedRoutedEventArgs args)
        {
            if (tabControl.SelectedItem == mathTab)
            {
                var item = (MathWorkspaceItemViewModel)this.mathControl.mathExpsListBox.SelectedItem;

                mathPresenter.Remove(item);
            }
            else if (tabControl.SelectedItem == logicTab)
            {
                var item = (LogicWorkspaceItemViewModel)this.logicControl.logicExpsListBox.SelectedItem;

                logicPresenter.Remove(item);
            }
            else if (tabControl.SelectedItem == graphsTab)
            {
                var item = (GraphItemViewModel)this.graphsControl.graphsList.SelectedItem;

                graphsPresenter.Remove(item);
            }
        }

        private void DeleteExp_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = (tabControl.SelectedItem == mathTab && this.mathControl.mathExpsListBox.SelectedItem != null) ||
                              (tabControl.SelectedItem == logicTab && this.logicControl.logicExpsListBox.SelectedItem != null) ||
                              (tabControl.SelectedItem == graphsTab && this.graphsControl.graphsList.SelectedItem != null);
        }

        private void Clear_Execute(object o, ExecutedRoutedEventArgs args)
        {
            if (tabControl.SelectedItem == mathTab)
                mathPresenter.Clear();
            else if (tabControl.SelectedItem == logicTab)
                logicPresenter.Clear();
            else if (tabControl.SelectedItem == graphsTab)
                graphsPresenter.Clear();
        }

        private void Clear_CanExecute(object o, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = tabControl.SelectedItem == mathTab ||
                              tabControl.SelectedItem == logicTab ||
                              tabControl.SelectedItem == graphsTab;
        }

        private void AboutCommand_Execute(object o, ExecutedRoutedEventArgs args)
        {
            AboutView aboutView = new AboutView { Owner = this };
            aboutView.ShowDialog();
        }

        #endregion Commands

        private TextBox GetSelectedTextBox()
        {
            if (tabControl.SelectedItem == mathTab)
                return this.mathControl.mathExpressionBox;
            if (tabControl.SelectedItem == logicTab)
                return this.logicControl.logicExpressionBox;
            if (tabControl.SelectedItem == graphsTab)
                return this.graphsControl.graphExpressionBox;
            if (tabControl.SelectedItem == truthTableTab)
                return this.truthTableControl.truthTableExpressionBox;

            return null;
        }

        private void InsertChar_Click(object o, RoutedEventArgs args)
        {
            var tag = ((Button)o).Tag.ToString();
            TextBox tb = GetSelectedTextBox();

            var prevSelectionStart = tb.SelectionStart;
            tb.Text = tb.Text.Insert(prevSelectionStart, tag);
            tb.SelectionStart = prevSelectionStart + tag.Length;
            tb.Focus();
        }

        private void InsertFunc_Click(object o, RoutedEventArgs args)
        {
            string func = ((Button)o).Tag.ToString();
            TextBox tb = GetSelectedTextBox();

            var prevSelectionStart = tb.SelectionStart;

            if (tb.SelectionLength > 0)
            {
                var prevSelectionLength = tb.SelectionLength;

                tb.Text = tb.Text.Insert(prevSelectionStart, func + "(").Insert(prevSelectionStart + prevSelectionLength + func.Length + 1, ")");
                tb.SelectionStart = prevSelectionStart + func.Length + prevSelectionLength + 2;
            }
            else
            {
                tb.Text = tb.Text.Insert(prevSelectionStart, func + "()");
                tb.SelectionStart = prevSelectionStart + func.Length + 1;
            }

            tb.Focus();
        }

        private void InsertInv_Click(object o, RoutedEventArgs args)
        {
            string func = ((Button)o).Tag.ToString();
            TextBox tb = GetSelectedTextBox();

            var prevSelectionStart = tb.SelectionStart;

            if (tb.SelectionLength > 0)
            {
                var prevSelectionLength = tb.SelectionLength;

                tb.Text = tb.Text.Insert(prevSelectionStart, "(").Insert(prevSelectionStart + prevSelectionLength + 1, ")" + func);
                tb.SelectionStart = prevSelectionStart + prevSelectionLength + func.Length + 2;
            }
            else
            {
                tb.Text = tb.Text.Insert(prevSelectionStart, func);
                tb.SelectionStart = prevSelectionStart + func.Length;
            }

            tb.Focus();
        }

        private void InsertDoubleArgFunc_Click(object o, RoutedEventArgs args)
        {
            string func = ((Button)o).Tag.ToString();
            TextBox tb = GetSelectedTextBox();

            var prevSelectionStart = tb.SelectionStart;

            if (tb.SelectionLength > 0)
            {
                var prevSelectionLength = tb.SelectionLength;

                tb.Text = tb.Text.Insert(prevSelectionStart, func + "(").Insert(prevSelectionStart + prevSelectionLength + func.Length + 1, ", )");
                tb.SelectionStart = prevSelectionStart + func.Length + prevSelectionLength + 3;
            }
            else
            {
                tb.Text = tb.Text.Insert(prevSelectionStart, func + "(, )");
                tb.SelectionStart = prevSelectionStart + func.Length + 1;
            }

            tb.Focus();
        }

        private void EnterButton_Click(object o, RoutedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(this.mathControl.mathExpressionBox.Text))
                this.mathControl.MathExpEnter();
        }

        private void tabControl_SelectionChanged(object o, SelectionChangedEventArgs args)
        {
            if (tabControl.SelectedItem == logicTab || tabControl.SelectedItem == truthTableTab)
            {
                numberToolBar.Visibility = Visibility.Collapsed;
                standartMathToolBar.Visibility = Visibility.Collapsed;
                trigonometricToolBar.Visibility = Visibility.Collapsed;
                hyperbolicToolBar.Visibility = Visibility.Collapsed;
                bitwiseToolBar.Visibility = Visibility.Collapsed;
                constantsMathToolBar.Visibility = Visibility.Collapsed;
                additionalMathToolBar.Visibility = Visibility.Collapsed;

                standartLogicToolBar.Visibility = Visibility.Visible;
                constantsLogicToolBar.Visibility = Visibility.Visible;
                additionalLogicToolBar.Visibility = Visibility.Visible;
            }
            else
            {
                numberToolBar.Visibility = Visibility.Visible;
                standartMathToolBar.Visibility = Visibility.Visible;
                trigonometricToolBar.Visibility = Visibility.Visible;
                hyperbolicToolBar.Visibility = Visibility.Visible;
                bitwiseToolBar.Visibility = Visibility.Visible;
                constantsMathToolBar.Visibility = Visibility.Visible;
                additionalMathToolBar.Visibility = Visibility.Visible;

                standartLogicToolBar.Visibility = Visibility.Collapsed;
                constantsLogicToolBar.Visibility = Visibility.Collapsed;
                additionalLogicToolBar.Visibility = Visibility.Collapsed;
            }
        }

    }

}
