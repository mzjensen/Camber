﻿#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acApp = Autodesk.AutoCAD.ApplicationServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using civDb = Autodesk.Civil.DatabaseServices;
using PressureExtension = Autodesk.Civil.DatabaseServices.Styles.StylesRootPressurePipesExtension;
using AeccPressurePartsList = Autodesk.Civil.DatabaseServices.Styles.PressurePartList;
using AeccPressurePartSize = Autodesk.Civil.DatabaseServices.Styles.PressurePartSize;
using DynamoServices;
using Camber.Civil.Styles;
#endregion

namespace Camber.Civil.PressureNetworks
{
    [RegisterForTrace]
    public sealed class PressurePartsList : Style
    {
        #region properties
        internal AeccPressurePartsList AeccPressurePartsList => AcObject as AeccPressurePartsList;
        #endregion

        #region constructors
        internal PressurePartsList(AeccPressurePartsList aeccPressurePartsList, bool isDynamoOwned = false) : base(aeccPressurePartsList, isDynamoOwned) { }

        internal static PressurePartsList GetByObjectId(acDb.ObjectId partsListId)
            => StyleSupport.Get<PressurePartsList, AeccPressurePartsList>
            (partsListId, (partsList) => new PressurePartsList(partsList));

        /// <summary>
        /// Creates a Pressure Parts List by name. Currently does not support element binding.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PressurePartsList ByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is null or empty.");
            }

            var document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                var partsListCollection = PressureExtension.GetPressurePartLists(cdoc.Styles);
                if (partsListCollection.Contains(name))
                {
                    throw new Exception("The document already has a Pressure Parts List with the same name.");
                }
                var id = partsListCollection.Add(name);
                return GetByObjectId(id);
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"PressurePartsList(Name = {Name})";

        /// <summary>
        /// Gets all of the Pressure Parts Lists in the document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static IList<PressurePartsList> GetPressurePartsLists(acDynNodes.Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            var res = new List<PressurePartsList>();
            using (var ctx = new acDynApp.DocumentContext((acApp.Document)null))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                var partsListCollection = PressureExtension.GetPressurePartLists(cdoc.Styles);
                foreach (acDb.ObjectId oid in partsListCollection)
                {
                    var obj = (AeccPressurePartsList)oid.GetObject(acDb.OpenMode.ForWrite);
                    if (obj != null)
                    {
                        res.Add(new PressurePartsList(obj, false));
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Gets the Pressure Parts in a Pressure Parts List by domain.
        /// </summary>
        /// <param name="pressurePartDomain"></param>
        /// <returns></returns>
        public IList<PressurePartSize> GetPartsByDomain(string pressurePartDomain)
        {
            List<PressurePartSize> parts = new List<PressurePartSize>();
            
            civDb.PressurePartDomainType domain = (civDb.PressurePartDomainType)Enum.Parse(
                typeof(civDb.PressurePartDomainType), 
                pressurePartDomain);

            List< AeccPressurePartSize> aeccParts = AeccPressurePartsList.GetParts(domain);
            
            foreach (AeccPressurePartSize aeccPart in aeccParts)
            {
                parts.Add(new PressurePartSize(aeccPart));
            }
            
            return parts;
        }

        /// <summary>
        /// Adds a Pressure Part to a Pressure Parts List.
        /// </summary>
        /// <param name="pressurePartSize"></param>
        /// <returns></returns>
        public PressurePartsList AddPart(PressurePartSize pressurePartSize)
        {
            try
            {
                AeccPressurePartsList.AddPart(pressurePartSize.AeccPressurePartSize);
                return this;
            } 
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            } 
        }

        /// <summary>
        /// Removes a Pressure Part from a Pressure Parts List.
        /// </summary>
        /// <param name="pressurePartSize"></param>
        /// <returns></returns>
        public PressurePartsList RemovePart(PressurePartSize pressurePartSize)
        {
            try
            {
                AeccPressurePartsList.RemovePart(pressurePartSize.AeccPressurePartSize);
                return this;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Updates a Pressure Part in a Pressure Parts List.
        /// </summary>
        /// <param name="pressurePartSize"></param>
        /// <returns></returns>
        public PressurePartsList UpdatePart(PressurePartSize pressurePartSize)
        {
            try
            {
                AeccPressurePartsList.UpdatePart(pressurePartSize.AeccPressurePartSize);
                return this;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
        #endregion
    }
}
