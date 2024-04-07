using AxMapWinGIS;
using MapWinGIS;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Tools;

namespace DynamicForms.Forms
{
    internal class EntityFormWithMap : Form, IEntityFormWithMap
    {
        public AxMapWinGIS.AxMap Map;
        private Button panBtn;
        private Button addShape;
        private Button ZoomIn;
        private Button ZoomOut;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        internal Shape Shape;
        private Shapefile _shapefile;
        private System.Windows.Forms.Label depth;
        private Button selectFromDict;
        private CheckBox customEntity;
        internal TabControl TabControl;
        internal TabPage PropertiesTab;
        internal TabPage LayersTab;
        internal TextBox Length;
        internal TextBox Angle;
        private string _shapefileFileName;

        internal object Entity { get; set; }
        public EntityFormWithMap(object entity)
        {
            Entity = entity;
            InitializeComponent();
            Map.CursorMode = tkCursorMode.cmPan;
            Map.SendMouseMove = true;

            FormClosed += (s, e) =>
            {
                if (_shapefile == null)
                {
                    return;
                }

                Map.Dispose();
                if (Directory.Exists(Path.GetDirectoryName(_shapefileFileName)))
                {
                    foreach (var file in Directory.EnumerateFiles(Path.GetDirectoryName(_shapefileFileName), $"{Path.GetFileNameWithoutExtension(_shapefileFileName)}.*"))
                    {
                        File.Delete(file);
                    }
                }
            };

            var prevLength = 0d;
            var prevAngle = 0d;

            Angle.GotFocus += (s, e) =>
            {
                prevAngle = TypeTools.Convert<double>(Angle.Text);
            };

            Angle.TextChanged += (s, e) =>
            {
                OnChangeParameters?.Invoke(_shapefile,
                    TypeTools.Convert<double>(Angle.Text), TypeTools.Convert<double>(Length.Text));

                var shape = _shapefile.NumShapes == 0 ? null : _shapefile.Shape[0];
                if (shape == null)
                {
                    return;
                }

                if (!callValidation(shape.Center, shape))
                {
                    OnChangeParameters?.Invoke(_shapefile, prevAngle, prevLength);
                }
                else
                {
                    AfterShapeValid?.Invoke(shape);
                }
            };

            Length.GotFocus += (s, e) =>
            {
                prevLength = TypeTools.Convert<double>(Length.Text);
            };

            Length.TextChanged += (s, e) =>
            {
                OnChangeParameters?.Invoke(_shapefile, 
                    TypeTools.Convert<double>(Angle.Text), TypeTools.Convert<double>(Length.Text));

                var shape = _shapefile.NumShapes == 0 ? null : _shapefile.Shape[0];
                if (shape == null)
                {
                    return;
                }
                if (!callValidation(shape.Center, shape))
                {
                    OnChangeParameters?.Invoke(_shapefile, prevAngle, prevLength);
                }
                else
                {
                    AfterShapeValid?.Invoke(shape);
                }
            };

            addShape.Enabled = customEntity.Checked || !Controls.ContainsKey(nameof(customEntity));
            customEntity.CheckedChanged += (s, e) =>
            {
                foreach (var text in PropertiesTab.Controls.OfType<TextBox>())
                {
                    if (text.Name.Equals("x", StringComparison.InvariantCultureIgnoreCase) ||
                        text.Name.Equals("y", StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    text.Enabled = customEntity.Checked;
                }

                addShape.Enabled = customEntity.Checked;
                if (!customEntity.Checked)
                {
                    Shape = null;
                    _shapefile.StartEditingShapes();
                    _shapefile.EditClear();
                    _shapefile.StopEditingShapes();
                }
            };
        }

        internal void MoveMapControls(int shift)
        {
            Map.Left += shift;
            Angle.Left += shift;
            Length.Left += shift;
            label1.Left += shift;
            label2.Left += shift;
            panBtn.Left += shift;
            addShape.Left += shift;
            ZoomIn.Left += shift;
            ZoomOut.Left += shift;
        }

        internal void CreateNewShapefile(Shapefile shapefile)
        {
            var guid = Guid.NewGuid();
            var directory = Path.Combine(Path.GetDirectoryName(shapefile.Filename), "Temp");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _shapefile = shapefile.Clone();
            _shapefileFileName = Path.Combine(directory, $"{guid}.shp");
            if (_shapefile.CreateNew(_shapefileFileName, shapefile.ShapefileType))
            {
                var layer = Map.AddLayer(_shapefile, true);
                Map.MoveLayerTop(layer);
            }
        }

        public T GetEntity<T>()
        {
            return TypeTools.Convert<T>(Entity);
        }
        public Shape GetShape()
        {
            return Shape?.Clone();
        }
        DialogResult IEntityForm.Activate()
        {
            var result = ShowDialog();
            return result;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntityFormWithMap));
            this.Map = new AxMapWinGIS.AxMap();
            this.panBtn = new System.Windows.Forms.Button();
            this.addShape = new System.Windows.Forms.Button();
            this.ZoomIn = new System.Windows.Forms.Button();
            this.ZoomOut = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.depth = new System.Windows.Forms.Label();
            this.selectFromDict = new System.Windows.Forms.Button();
            this.customEntity = new System.Windows.Forms.CheckBox();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.PropertiesTab = new System.Windows.Forms.TabPage();
            this.LayersTab = new System.Windows.Forms.TabPage();
            this.Length = new System.Windows.Forms.TextBox();
            this.Angle = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.Map)).BeginInit();
            this.TabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // Map
            // 
            this.Map.Enabled = true;
            this.Map.Location = new System.Drawing.Point(202, 35);
            this.Map.Name = "Map";
            this.Map.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("Map.OcxState")));
            this.Map.Size = new System.Drawing.Size(680, 366);
            this.Map.TabIndex = 0;
            this.Map.MouseDownEvent += new AxMapWinGIS._DMapEvents_MouseDownEventHandler(this.Map_MouseDownEvent);
            this.Map.MouseMoveEvent += new AxMapWinGIS._DMapEvents_MouseMoveEventHandler(this.Map_MouseMoveEvent);
            // 
            // panBtn
            // 
            this.panBtn.Location = new System.Drawing.Point(202, 6);
            this.panBtn.Name = "panBtn";
            this.panBtn.Size = new System.Drawing.Size(75, 23);
            this.panBtn.TabIndex = 1;
            this.panBtn.Text = "Pan";
            this.panBtn.UseVisualStyleBackColor = true;
            this.panBtn.Click += new System.EventHandler(this.panBtn_Click);
            // 
            // addShape
            // 
            this.addShape.Location = new System.Drawing.Point(283, 8);
            this.addShape.Name = "addShape";
            this.addShape.Size = new System.Drawing.Size(75, 21);
            this.addShape.TabIndex = 2;
            this.addShape.Text = "Add shape";
            this.addShape.UseVisualStyleBackColor = true;
            this.addShape.Click += new System.EventHandler(this.addShape_Click);
            // 
            // ZoomIn
            // 
            this.ZoomIn.Location = new System.Drawing.Point(364, 6);
            this.ZoomIn.Name = "ZoomIn";
            this.ZoomIn.Size = new System.Drawing.Size(75, 23);
            this.ZoomIn.TabIndex = 3;
            this.ZoomIn.Text = "Zoom In";
            this.ZoomIn.UseVisualStyleBackColor = true;
            this.ZoomIn.Click += new System.EventHandler(this.ZoomIn_Click);
            // 
            // ZoomOut
            // 
            this.ZoomOut.Location = new System.Drawing.Point(445, 6);
            this.ZoomOut.Name = "ZoomOut";
            this.ZoomOut.Size = new System.Drawing.Size(75, 23);
            this.ZoomOut.TabIndex = 4;
            this.ZoomOut.Text = "Zoom Out";
            this.ZoomOut.UseVisualStyleBackColor = true;
            this.ZoomOut.Click += new System.EventHandler(this.ZoomOut_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(204, 405);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Angle";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(310, 405);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Length";
            // 
            // depth
            // 
            this.depth.AutoSize = true;
            this.depth.Location = new System.Drawing.Point(768, 12);
            this.depth.Name = "depth";
            this.depth.Size = new System.Drawing.Size(42, 13);
            this.depth.TabIndex = 9;
            this.depth.Text = "Depth: ";
            // 
            // selectFromDict
            // 
            this.selectFromDict.Location = new System.Drawing.Point(12, 35);
            this.selectFromDict.Name = "selectFromDict";
            this.selectFromDict.Size = new System.Drawing.Size(122, 23);
            this.selectFromDict.TabIndex = 10;
            this.selectFromDict.Text = "Select from dictionary";
            this.selectFromDict.UseVisualStyleBackColor = true;
            this.selectFromDict.Click += new System.EventHandler(this.selectFromDict_Click);
            // 
            // customEntity
            // 
            this.customEntity.AutoSize = true;
            this.customEntity.Location = new System.Drawing.Point(13, 65);
            this.customEntity.Name = "customEntity";
            this.customEntity.Size = new System.Drawing.Size(89, 17);
            this.customEntity.TabIndex = 11;
            this.customEntity.Text = "Custom entity";
            this.customEntity.UseVisualStyleBackColor = true;
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.PropertiesTab);
            this.TabControl.Controls.Add(this.LayersTab);
            this.TabControl.Location = new System.Drawing.Point(13, 89);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(183, 312);
            this.TabControl.TabIndex = 12;
            // 
            // PropertiesTab
            // 
            this.PropertiesTab.Location = new System.Drawing.Point(4, 22);
            this.PropertiesTab.Name = "PropertiesTab";
            this.PropertiesTab.Padding = new System.Windows.Forms.Padding(3);
            this.PropertiesTab.Size = new System.Drawing.Size(175, 286);
            this.PropertiesTab.TabIndex = 0;
            this.PropertiesTab.Text = "Properties";
            this.PropertiesTab.UseVisualStyleBackColor = true;
            this.PropertiesTab.Click += new System.EventHandler(this.PropertiesTab_Click);
            // 
            // LayersTab
            // 
            this.LayersTab.Location = new System.Drawing.Point(4, 22);
            this.LayersTab.Name = "LayersTab";
            this.LayersTab.Padding = new System.Windows.Forms.Padding(3);
            this.LayersTab.Size = new System.Drawing.Size(175, 286);
            this.LayersTab.TabIndex = 1;
            this.LayersTab.Text = "Layers manager";
            this.LayersTab.UseVisualStyleBackColor = true;
            // 
            // length
            // 
            this.Length.Location = new System.Drawing.Point(313, 421);
            this.Length.Name = "length";
            this.Length.Size = new System.Drawing.Size(100, 20);
            this.Length.TabIndex = 13;
            // 
            // angle
            // 
            this.Angle.Location = new System.Drawing.Point(202, 421);
            this.Angle.Name = "angle";
            this.Angle.Size = new System.Drawing.Size(100, 20);
            this.Angle.TabIndex = 14;
            this.Angle.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // EntityFormWithMap
            // 
            this.ClientSize = new System.Drawing.Size(894, 466);
            this.Controls.Add(this.Angle);
            this.Controls.Add(this.Length);
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.customEntity);
            this.Controls.Add(this.selectFromDict);
            this.Controls.Add(this.depth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ZoomOut);
            this.Controls.Add(this.ZoomIn);
            this.Controls.Add(this.addShape);
            this.Controls.Add(this.panBtn);
            this.Controls.Add(this.Map);
            this.Name = "EntityFormWithMap";
            ((System.ComponentModel.ISupportInitialize)(this.Map)).EndInit();
            this.TabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        internal bool SendMouseDownEvent
        {
            set => Map.SendMouseDown = value;
            get => Map.SendMouseDown;
        }

        internal event Action<Point> OnMapMouseDown;
        internal event Func<Point, Shape, bool> ValidShape;
        internal event Action<Shape> AfterShapeValid;
        internal event Action<Shapefile, double, double> OnChangeParameters;
        internal event Action<double, double> OnMouseMoveOnMap;
        internal event Action OnSelectFromDictionary;

        internal void HideAngleAndLength()
        {
            label1.Visible = false;
            label2.Visible = false;
            Angle.Visible = false;
            Length.Visible = false;
        }

        private void ZoomOut_Click(object sender, System.EventArgs e)
        {
            Map.CursorMode = tkCursorMode.cmZoomOut;
        }

        private void panBtn_Click(object sender, System.EventArgs e)
        {
            Map.CursorMode = tkCursorMode.cmPan;
        }

        private void addShape_Click(object sender, System.EventArgs e)
        {
            Map.CursorMode = tkCursorMode.cmAddShape;
        }

        private void ZoomIn_Click(object sender, System.EventArgs e)
        {
            Map.CursorMode = tkCursorMode.cmZoomIn;
        }

        private void Map_MouseDownEvent(object sender, _DMapEvents_MouseDownEvent e)
        {
            var projX = 0d;
            var projY = 0d;
            Map.PixelToProj(e.x, e.y, ref projX, ref projY);

            var point = new Point();
            point.x = projX;
            point.y = projY;

            OnMapMouseDown?.Invoke(point);

            if (Map.CursorMode == tkCursorMode.cmAddShape)
            {
                if (Shape == null)
                {
                    CreateShape();
                }
                    
                if (!callValidation(point, Shape))
                {
                    MessageBox.Show("Invalid shape");
                }
                else
                {
                    InsertPoint(point);

                    AfterShapeValid?.Invoke(Shape);
                }

                Map.Redraw();
            }
        }

        private bool callValidation(Point point, Shape shape)
        {
            var validFuncs = ValidShape?.GetInvocationList().OfType<Func<Point, Shape, bool>>()
                ?? Enumerable.Empty<Func<Point, Shape, bool>>();
            foreach (var func in validFuncs)
            {
                if (!func.Invoke(point, shape))
                {
                    return false;
                }
            }

            return true;
        }

        private void Map_MouseMoveEvent(object sender, _DMapEvents_MouseMoveEvent e)
        {
            OnMouseMoveOnMap?.Invoke(e.x, e.y);
        }

        private void selectFromDict_Click(object sender, EventArgs e)
        {
            OnSelectFromDictionary?.Invoke();
        }

        internal Shape CreateShape()
        {
            _shapefile.StartAppendMode();
            var shape = new Shape();
            shape.Create(_shapefile.ShapefileType);
            Shape = shape;

            var shapeIndex = 0;
            _shapefile.EditInsertShape(Shape, ref shapeIndex);
            _shapefile.StopAppendMode();

            return Shape;
        }

        internal void InsertPoint(Point point)
        {
            var pointIndex = 0;
            var isPoint = _shapefile.ShapefileType == ShpfileType.SHP_POINT || _shapefile.ShapefileType == ShpfileType.SHP_POINTZ;
            if (isPoint && Shape.numPoints == 1)
            {
                Shape.Point[0] = point;
            }
            else
            {
                Shape.InsertPoint(point, ref pointIndex);
            }
        }

        private void PropertiesTab_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
