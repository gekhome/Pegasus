namespace Pegasus.Reports.Misc
{
    partial class NumberAitiseisDaily
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.GraphGroup graphGroup1 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.CategoryScale categoryScale1 = new Telerik.Reporting.CategoryScale();
            Telerik.Reporting.NumericalScale numericalScale1 = new Telerik.Reporting.NumericalScale();
            Telerik.Reporting.GraphGroup graphGroup2 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.TypeReportSource typeReportSource1 = new Telerik.Reporting.TypeReportSource();
            Telerik.Reporting.Group group1 = new Telerik.Reporting.Group();
            Telerik.Reporting.Group group2 = new Telerik.Reporting.Group();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            this.PROSKLISIGroupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.cATEGORY_TEXTCountFunctionTextBox = new Telerik.Reporting.TextBox();
            this.graph1 = new Telerik.Reporting.Graph();
            this.cartesianCoordinateSystem1 = new Telerik.Reporting.CartesianCoordinateSystem();
            this.graphAxis1 = new Telerik.Reporting.GraphAxis();
            this.graphAxis2 = new Telerik.Reporting.GraphAxis();
            this.sqlDataSource = new Telerik.Reporting.SqlDataSource();
            this.lineSeries1 = new Telerik.Reporting.LineSeries();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.Û◊œÀ… œ_≈‘œ”GroupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.Û◊œÀ_≈‘œ”DataTextBox = new Telerik.Reporting.TextBox();
            this.Û◊œÀ_≈‘œ”CaptionTextBox = new Telerik.Reporting.TextBox();
            this.labelsGroupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.labelsGroupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.sqlProkirixeis = new Telerik.Reporting.SqlDataSource();
            this.pageFooter = new Telerik.Reporting.PageFooterSection();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.pageInfoTextBox = new Telerik.Reporting.TextBox();
            this.currentTimeTextBox = new Telerik.Reporting.TextBox();
            this.reportHeader = new Telerik.Reporting.ReportHeaderSection();
            this.subReport1 = new Telerik.Reporting.SubReport();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // PROSKLISIGroupFooterSection
            // 
            this.PROSKLISIGroupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Cm(15.932493209838867D);
            this.PROSKLISIGroupFooterSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.cATEGORY_TEXTCountFunctionTextBox,
            this.graph1,
            this.textBox2});
            this.PROSKLISIGroupFooterSection.Name = "PROSKLISIGroupFooterSection";
            this.PROSKLISIGroupFooterSection.Style.BackgroundColor = System.Drawing.Color.LightGray;
            // 
            // cATEGORY_TEXTCountFunctionTextBox
            // 
            this.cATEGORY_TEXTCountFunctionTextBox.CanGrow = true;
            this.cATEGORY_TEXTCountFunctionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0.00010002215276472271D), Telerik.Reporting.Drawing.Unit.Cm(0.10573320835828781D));
            this.cATEGORY_TEXTCountFunctionTextBox.Name = "cATEGORY_TEXTCountFunctionTextBox";
            this.cATEGORY_TEXTCountFunctionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(17.718547821044922D), Telerik.Reporting.Drawing.Unit.Cm(0.64739948511123657D));
            this.cATEGORY_TEXTCountFunctionTextBox.Style.Font.Bold = true;
            this.cATEGORY_TEXTCountFunctionTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.cATEGORY_TEXTCountFunctionTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.cATEGORY_TEXTCountFunctionTextBox.StyleName = "Data";
            this.cATEGORY_TEXTCountFunctionTextBox.Value = "= \"«Ã≈—«”…¡ Ã≈‘¡¬œÀ« ¡…‘«”≈ŸÕ –—œ «—’Œ«” : \" + Fields.PROKIRIXI_PROTOCOL";
            // 
            // graph1
            // 
            graphGroup1.Groupings.Add(new Telerik.Reporting.Grouping("=Fields.AITISI_DATE"));
            graphGroup1.Label = "= Format(\'{0:dd/MM}\', Fields.AITISI_DATE)";
            graphGroup1.Name = "categoryGroup";
            graphGroup1.Sortings.Add(new Telerik.Reporting.Sorting("=Fields.AITISI_DATE", Telerik.Reporting.SortDirection.Asc));
            this.graph1.CategoryGroups.Add(graphGroup1);
            this.graph1.CoordinateSystems.Add(this.cartesianCoordinateSystem1);
            this.graph1.DataSource = this.sqlDataSource;
            this.graph1.Filters.Add(new Telerik.Reporting.Filter("=Fields.PROKIRIXI_D", Telerik.Reporting.FilterOperator.Equal, "=Parameters.prokirixiID.Value"));
            this.graph1.Legend.IsInsidePlotArea = true;
            this.graph1.Legend.Position = Telerik.Reporting.GraphItemPosition.TopCenter;
            this.graph1.Legend.Style.LineColor = System.Drawing.Color.LightGray;
            this.graph1.Legend.Style.LineWidth = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.graph1.Legend.Style.Visible = true;
            this.graph1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0.30000004172325134D), Telerik.Reporting.Drawing.Unit.Cm(1.3324912786483765D));
            this.graph1.Name = "graph1";
            this.graph1.PlotAreaStyle.LineColor = System.Drawing.Color.LightGray;
            this.graph1.PlotAreaStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.graph1.Series.Add(this.lineSeries1);
            this.graph1.SeriesGroups.Add(graphGroup2);
            this.graph1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(17.200000762939453D), Telerik.Reporting.Drawing.Unit.Cm(12.946866989135742D));
            this.graph1.Style.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            // 
            // cartesianCoordinateSystem1
            // 
            this.cartesianCoordinateSystem1.Name = "cartesianCoordinateSystem1";
            this.cartesianCoordinateSystem1.XAxis = this.graphAxis1;
            this.cartesianCoordinateSystem1.YAxis = this.graphAxis2;
            // 
            // graphAxis1
            // 
            this.graphAxis1.LabelAngle = 45;
            this.graphAxis1.MajorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis1.MajorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis1.MinorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis1.MinorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis1.MinorGridLineStyle.Visible = false;
            this.graphAxis1.Name = "GraphAxis1";
            this.graphAxis1.Scale = categoryScale1;
            this.graphAxis1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.graphAxis1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.graphAxis1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            // 
            // graphAxis2
            // 
            this.graphAxis2.MajorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis2.MajorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis2.MinorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis2.MinorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis2.MinorGridLineStyle.Visible = false;
            this.graphAxis2.MinorTickMarkDisplayType = Telerik.Reporting.GraphAxisTickMarkDisplayType.Inside;
            this.graphAxis2.Name = "GraphAxis2";
            numericalScale1.Minimum = 0D;
            this.graphAxis2.Scale = numericalScale1;
            // 
            // sqlDataSource
            // 
            this.sqlDataSource.ConnectionString = "Pegasus.Properties.Settings.DBConnectionString";
            this.sqlDataSource.Name = "sqlDataSource";
            this.sqlDataSource.SelectCommand = "SELECT        PROKIRIXI_D, PROKIRIXI_PROTOCOL, AITISI_DATE, –À«»œ”\r\nFROM         " +
    "   sqlAITISEIS_PER_DAY\r\nORDER BY AITISI_DATE";
            // 
            // lineSeries1
            // 
            this.lineSeries1.ArrangeByAxis = this.graphAxis1;
            this.lineSeries1.CategoryGroup = graphGroup1;
            this.lineSeries1.CoordinateSystem = this.cartesianCoordinateSystem1;
            this.lineSeries1.DataPointLabel = "= Fields.–À«»œ”";
            this.lineSeries1.DataPointLabelStyle.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.lineSeries1.DataPointLabelStyle.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Cm(0.20000000298023224D);
            this.lineSeries1.DataPointLabelStyle.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.lineSeries1.DataPointStyle.Visible = true;
            this.lineSeries1.LegendItem.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.lineSeries1.LegendItem.Style.LineColor = System.Drawing.Color.Transparent;
            this.lineSeries1.LegendItem.Style.LineWidth = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.lineSeries1.LineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(2D);
            this.lineSeries1.LineType = Telerik.Reporting.LineSeries.LineTypes.Smooth;
            this.lineSeries1.MarkerMaxSize = Telerik.Reporting.Drawing.Unit.Pixel(50D);
            this.lineSeries1.MarkerMinSize = Telerik.Reporting.Drawing.Unit.Pixel(5D);
            this.lineSeries1.MarkerSize = Telerik.Reporting.Drawing.Unit.Pixel(8D);
            this.lineSeries1.MarkerType = Telerik.Reporting.DataPointMarkerType.Circle;
            graphGroup2.Name = "seriesGroup";
            this.lineSeries1.SeriesGroup = graphGroup2;
            this.lineSeries1.Size = null;
            this.lineSeries1.Y = "= Fields.–À«»œ”";
            // 
            // textBox2
            // 
            this.textBox2.CanGrow = true;
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0.30000025033950806D), Telerik.Reporting.Drawing.Unit.Cm(14.832491874694824D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(17.200000762939453D), Telerik.Reporting.Drawing.Unit.Cm(0.64708298444747925D));
            this.textBox2.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.Style.Font.Name = "Calibri";
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox2.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Pixel(4D);
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox2.StyleName = "Data";
            this.textBox2.Value = "= \"Ã≈”œ” ¡—…»Ãœ” ¡…‘«”≈ŸÕ/«Ã≈—¡ = \" + CStr(Sum(Fields.–À«»œ”)/Count(Fields.AITISI" +
    "_DATE))";
            // 
            // Û◊œÀ… œ_≈‘œ”GroupHeaderSection
            // 
            this.Û◊œÀ… œ_≈‘œ”GroupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Cm(0.70000004768371582D);
            this.Û◊œÀ… œ_≈‘œ”GroupHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.Û◊œÀ_≈‘œ”DataTextBox,
            this.Û◊œÀ_≈‘œ”CaptionTextBox});
            this.Û◊œÀ… œ_≈‘œ”GroupHeaderSection.Name = "Û◊œÀ… œ_≈‘œ”GroupHeaderSection";
            this.Û◊œÀ… œ_≈‘œ”GroupHeaderSection.Style.BackgroundColor = System.Drawing.Color.LightGray;
            // 
            // Û◊œÀ_≈‘œ”DataTextBox
            // 
            this.Û◊œÀ_≈‘œ”DataTextBox.CanGrow = true;
            this.Û◊œÀ_≈‘œ”DataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(2.700200080871582D), Telerik.Reporting.Drawing.Unit.Cm(0.00010012308484874666D));
            this.Û◊œÀ_≈‘œ”DataTextBox.Name = "Û◊œÀ_≈‘œ”DataTextBox";
            this.Û◊œÀ_≈‘œ”DataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(15.1410493850708D), Telerik.Reporting.Drawing.Unit.Cm(0.64708298444747925D));
            this.Û◊œÀ_≈‘œ”DataTextBox.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.Û◊œÀ_≈‘œ”DataTextBox.Style.Font.Bold = true;
            this.Û◊œÀ_≈‘œ”DataTextBox.Style.Font.Name = "Calibri";
            this.Û◊œÀ_≈‘œ”DataTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.Û◊œÀ_≈‘œ”DataTextBox.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Pixel(4D);
            this.Û◊œÀ_≈‘œ”DataTextBox.StyleName = "Data";
            this.Û◊œÀ_≈‘œ”DataTextBox.Value = "= Fields.PROKIRIXI_PROTOCOL";
            // 
            // Û◊œÀ_≈‘œ”CaptionTextBox
            // 
            this.Û◊œÀ_≈‘œ”CaptionTextBox.CanGrow = true;
            this.Û◊œÀ_≈‘œ”CaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0D), Telerik.Reporting.Drawing.Unit.Cm(0.00010012308484874666D));
            this.Û◊œÀ_≈‘œ”CaptionTextBox.Name = "Û◊œÀ_≈‘œ”CaptionTextBox";
            this.Û◊œÀ_≈‘œ”CaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(2.6999998092651367D), Telerik.Reporting.Drawing.Unit.Cm(0.64708298444747925D));
            this.Û◊œÀ_≈‘œ”CaptionTextBox.Style.BackgroundColor = System.Drawing.Color.Empty;
            this.Û◊œÀ_≈‘œ”CaptionTextBox.Style.Font.Bold = true;
            this.Û◊œÀ_≈‘œ”CaptionTextBox.Style.Font.Name = "Calibri";
            this.Û◊œÀ_≈‘œ”CaptionTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.Û◊œÀ_≈‘œ”CaptionTextBox.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Pixel(4D);
            this.Û◊œÀ_≈‘œ”CaptionTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.Û◊œÀ_≈‘œ”CaptionTextBox.StyleName = "Caption";
            this.Û◊œÀ_≈‘œ”CaptionTextBox.Value = "–Ò¸ÛÍÎÁÛÁ:";
            // 
            // labelsGroupFooterSection
            // 
            this.labelsGroupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Cm(0.36750823259353638D);
            this.labelsGroupFooterSection.Name = "labelsGroupFooterSection";
            this.labelsGroupFooterSection.Style.Visible = false;
            // 
            // labelsGroupHeaderSection
            // 
            this.labelsGroupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Cm(0.29999983310699463D);
            this.labelsGroupHeaderSection.Name = "labelsGroupHeaderSection";
            this.labelsGroupHeaderSection.PrintOnEveryPage = true;
            this.labelsGroupHeaderSection.Style.Visible = true;
            // 
            // sqlProkirixeis
            // 
            this.sqlProkirixeis.ConnectionString = "Pegasus.Properties.Settings.DBConnectionString";
            this.sqlProkirixeis.Name = "sqlProkirixeis";
            this.sqlProkirixeis.SelectCommand = "SELECT        ID, PROTOCOL\r\nFROM            PROKIRIXIS\r\nORDER BY DATE_START DESC";
            // 
            // pageFooter
            // 
            this.pageFooter.Height = Telerik.Reporting.Drawing.Unit.Cm(1.0357164144515991D);
            this.pageFooter.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox16,
            this.textBox20,
            this.pageInfoTextBox,
            this.currentTimeTextBox});
            this.pageFooter.Name = "pageFooter";
            // 
            // textBox16
            // 
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(8.8999996185302734D), Telerik.Reporting.Drawing.Unit.Cm(0.00010012308484874666D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(8.8285512924194336D), Telerik.Reporting.Drawing.Unit.Cm(0.45978257060050964D));
            this.textBox16.Style.Font.Name = "Calibri";
            this.textBox16.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox16.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox16.StyleName = "PageInfo";
            this.textBox16.Value = "ƒ…≈’»’Õ”« ≈–¡√√≈ÀÃ¡‘… «”  ¡‘¡—‘…”«”";
            // 
            // textBox20
            // 
            this.textBox20.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0D), Telerik.Reporting.Drawing.Unit.Cm(0.00010012308484874666D));
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(4.8412480354309082D), Telerik.Reporting.Drawing.Unit.Cm(0.45978257060050964D));
            this.textBox20.Style.Font.Name = "Calibri";
            this.textBox20.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox20.StyleName = "PageInfo";
            this.textBox20.Value = "≈÷¡—Ãœ√« Pegasus";
            // 
            // pageInfoTextBox
            // 
            this.pageInfoTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(8.8999996185302734D), Telerik.Reporting.Drawing.Unit.Cm(0.4763500988483429D));
            this.pageInfoTextBox.Name = "pageInfoTextBox";
            this.pageInfoTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(8.8186502456665039D), Telerik.Reporting.Drawing.Unit.Cm(0.55936628580093384D));
            this.pageInfoTextBox.Style.Font.Name = "Calibri";
            this.pageInfoTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.pageInfoTextBox.StyleName = "PageInfo";
            this.pageInfoTextBox.Value = "=\"”ÂÎ. \" + PageNumber + \"/\" + PageCount";
            // 
            // currentTimeTextBox
            // 
            this.currentTimeTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0D), Telerik.Reporting.Drawing.Unit.Cm(0.4763500988483429D));
            this.currentTimeTextBox.Name = "currentTimeTextBox";
            this.currentTimeTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(4.8412480354309082D), Telerik.Reporting.Drawing.Unit.Cm(0.55936628580093384D));
            this.currentTimeTextBox.Style.Font.Name = "Calibri";
            this.currentTimeTextBox.StyleName = "PageInfo";
            this.currentTimeTextBox.Value = "=NOW()";
            // 
            // reportHeader
            // 
            this.reportHeader.Height = Telerik.Reporting.Drawing.Unit.Cm(4D);
            this.reportHeader.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.subReport1,
            this.textBox1});
            this.reportHeader.Name = "reportHeader";
            // 
            // subReport1
            // 
            this.subReport1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0D), Telerik.Reporting.Drawing.Unit.Cm(0D));
            this.subReport1.Name = "subReport1";
            typeReportSource1.TypeName = "Pegasus.Reports.A2Logo, Pegasus, Version=1.0.0.0, Culture=neutral, PublicKeyToken" +
    "=null";
            this.subReport1.ReportSource = typeReportSource1;
            this.subReport1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(10.100000381469727D), Telerik.Reporting.Drawing.Unit.Cm(3.1997997760772705D));
            // 
            // textBox1
            // 
            this.textBox1.CanGrow = true;
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0D), Telerik.Reporting.Drawing.Unit.Cm(3.2000000476837158D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(17.841251373291016D), Telerik.Reporting.Drawing.Unit.Cm(0.70000004768371582D));
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Font.Name = "Calibri";
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox1.StyleName = "Caption";
            this.textBox1.Value = "«Ã≈—«”…¡ Ã≈‘¡¬œÀ« ¡…‘«”≈ŸÕ Ÿ—œÃ…”»…ŸÕ ≈ –¡…ƒ≈’‘… ŸÕ ”‘¡ …≈  - ƒ’–¡";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Cm(0.30000022053718567D);
            this.detail.Name = "detail";
            this.detail.Style.Visible = false;
            // 
            // NumberAitiseisDaily
            // 
            this.DataSource = this.sqlDataSource;
            this.Filters.Add(new Telerik.Reporting.Filter("=Fields.PROKIRIXI_D", Telerik.Reporting.FilterOperator.Equal, "=Parameters.prokirixiID.Value"));
            group1.GroupFooter = this.PROSKLISIGroupFooterSection;
            group1.GroupHeader = this.Û◊œÀ… œ_≈‘œ”GroupHeaderSection;
            group1.Groupings.Add(new Telerik.Reporting.Grouping("=Fields.PROKIRIXI_PROTOCOL"));
            group1.Name = "PROKIRIXIGroup";
            group2.GroupFooter = this.labelsGroupFooterSection;
            group2.GroupHeader = this.labelsGroupHeaderSection;
            group2.Name = "labelsGroup";
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            group1,
            group2});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.Û◊œÀ… œ_≈‘œ”GroupHeaderSection,
            this.PROSKLISIGroupFooterSection,
            this.labelsGroupHeaderSection,
            this.labelsGroupFooterSection,
            this.pageFooter,
            this.reportHeader,
            this.detail});
            this.Name = "NumberAitiseisDaily";
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Mm(20D), Telerik.Reporting.Drawing.Unit.Mm(10D), Telerik.Reporting.Drawing.Unit.Mm(20D), Telerik.Reporting.Drawing.Unit.Mm(20D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AllowNull = true;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.sqlProkirixeis;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.PROTOCOL";
            reportParameter1.AvailableValues.ValueMember = "= Fields.ID";
            reportParameter1.Name = "prokirixiID";
            reportParameter1.Text = "–ÒÔÍﬁÒıÓÁ";
            reportParameter1.Type = Telerik.Reporting.ReportParameterType.Integer;
            reportParameter1.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Title")});
            styleRule1.Style.Color = System.Drawing.Color.Black;
            styleRule1.Style.Font.Bold = true;
            styleRule1.Style.Font.Italic = false;
            styleRule1.Style.Font.Name = "Tahoma";
            styleRule1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(18D);
            styleRule1.Style.Font.Strikeout = false;
            styleRule1.Style.Font.Underline = false;
            styleRule2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Caption")});
            styleRule2.Style.Color = System.Drawing.Color.Black;
            styleRule2.Style.Font.Name = "Tahoma";
            styleRule2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            styleRule2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Data")});
            styleRule3.Style.Font.Name = "Tahoma";
            styleRule3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            styleRule3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("PageInfo")});
            styleRule4.Style.Font.Name = "Tahoma";
            styleRule4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            styleRule4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1,
            styleRule2,
            styleRule3,
            styleRule4});
            this.Width = Telerik.Reporting.Drawing.Unit.Cm(17.841251373291016D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.SqlDataSource sqlDataSource;
        private Telerik.Reporting.GroupHeaderSection Û◊œÀ… œ_≈‘œ”GroupHeaderSection;
        private Telerik.Reporting.GroupFooterSection PROSKLISIGroupFooterSection;
        private Telerik.Reporting.TextBox cATEGORY_TEXTCountFunctionTextBox;
        private Telerik.Reporting.GroupHeaderSection labelsGroupHeaderSection;
        private Telerik.Reporting.GroupFooterSection labelsGroupFooterSection;
        private Telerik.Reporting.PageFooterSection pageFooter;
        private Telerik.Reporting.ReportHeaderSection reportHeader;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox Û◊œÀ_≈‘œ”DataTextBox;
        private Telerik.Reporting.TextBox Û◊œÀ_≈‘œ”CaptionTextBox;
        private Telerik.Reporting.SubReport subReport1;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox pageInfoTextBox;
        private Telerik.Reporting.TextBox currentTimeTextBox;
        private Telerik.Reporting.Graph graph1;
        private Telerik.Reporting.CartesianCoordinateSystem cartesianCoordinateSystem1;
        private Telerik.Reporting.GraphAxis graphAxis1;
        private Telerik.Reporting.GraphAxis graphAxis2;
        private Telerik.Reporting.LineSeries lineSeries1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.SqlDataSource sqlProkirixeis;

    }
}