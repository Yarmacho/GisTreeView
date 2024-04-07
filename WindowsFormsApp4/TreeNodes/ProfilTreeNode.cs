//using Entities;
//using Entities.Entities;
//using MapWinGIS;
//using System;
//using WindowsFormsApp4.TreeNodes.Abstractions;

//namespace WindowsFormsApp4.TreeNodes
//{
//    internal class ProfilTreeNode : ShapeTreeNode<Profil>
//    {
//        public ProfilTreeNode(Shapefile shapefile, int shapeIndex, int layerHandle) 
//            : base(shapefile, shapeIndex, layerHandle)
//        {
//            var nameFieldIndex = Shapefile.FieldIndexByName["Id"];
//            if (nameFieldIndex == -1)
//            {
//                throw new ArgumentException("Incorrent shapefile provided!");
//            }
//            Name = $"Profil {Shapefile.CellValue[nameFieldIndex, shapeIndex]?.ToString()}";
//            Text = $"Profil {Shapefile.CellValue[nameFieldIndex, shapeIndex]?.ToString()}";
//        }

//        protected override void ConfigureChildNodeEntity(object childEntity)
//        {
//        }

//        protected override void OnUpdate(Profil entity)
//        {
//            Name = $"Profil {entity.Name}";
//            Text = $"Profil {entity.Name}";
//        }
//    }
//}
