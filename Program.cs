using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System.Collections.Generic;

namespace ReplaceLinesWithBlocks
{
    public class Commands
    {
        [CommandMethod("ReplaceLinesWithBlocks")]
        public void ReplaceLinesWithBlocks()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                // Prompt user to select lines
                PromptSelectionOptions opts = new PromptSelectionOptions();
                opts.MessageForAdding = "Select lines to replace with blocks";
                PromptSelectionResult selRes = ed.GetSelection(opts);

                if (selRes.Status != PromptStatus.OK)
                {
                    ed.WriteMessage("No lines selected.");
                    trans.Abort();
                    return;
                }

                SelectionSet selSet = selRes.Value;
                List<ObjectId> horizontalLines = new List<ObjectId>();
                List<ObjectId> verticalLines = new List<ObjectId>();

                // Categorize lines as horizontal or vertical
                foreach (SelectedObject selObj in selSet)
                {
                    if (selObj != null)
                    {
                        Line line = trans.GetObject(selObj.ObjectId, OpenMode.ForRead) as Line;
                        if (line != null)
                        {
                            if (IsHorizontal(line))
                            {
                                horizontalLines.Add(line.ObjectId);
                            }
                            else if (IsVertical(line))
                            {
                                verticalLines.Add(line.ObjectId);
                            }
                        }
                    }
                }

                ed.WriteMessage("\nnumber of vertical lines: " + verticalLines.Count);

                // Replace vertical lines with "TM[1]Staander MP 200" blocks
                foreach (ObjectId lineId in verticalLines)
                {
                    Line line = trans.GetObject(lineId, OpenMode.ForWrite) as Line;
                    if (line != null)
                    {
                        double length = line.Length;
                        if (length == 2000)
                        {
                            ReplaceLineWithBlock(line, "TM[1]Staander MP 200", new Vector3d(0, 0, 97), trans);
                            line.Erase();
                        }
                    }
                }

                // Replace horizontal lines with appropriate blocks
                foreach (ObjectId lineId in horizontalLines)
                {
                    Line line = trans.GetObject(lineId, OpenMode.ForWrite) as Line;
                    if (line != null)
                    {
                        double length = line.Length;
                        if (length == 2572)
                        {
                            ReplaceLineWithBlock(line, "TM[1]Ligger 257", new Vector3d(), trans);
                        }
                        else if (length == 1572)
                        {
                            ReplaceLineWithBlock(line, "TM[1]Ligger 157", new Vector3d(), trans);
                        }
                        else if (length == 1088)
                        {
                            ReplaceLineWithBlock(line, "TM[1]Ligger 109", new Vector3d(), trans);
                        }
                        line.Erase();
                    }
                }

                trans.Commit();
            }
        }

        private bool IsHorizontal(Line line)
        {
            return line.StartPoint.Z == line.EndPoint.Z;
        }

        private bool IsVertical(Line line)
        {
            //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nline coords: " + line.StartPoint.X +"+++"+ line.EndPoint.X);
            return line.StartPoint.X == line.EndPoint.X && line.StartPoint.Y == line.EndPoint.Y;
        }

        private void ReplaceLineWithBlock(Line line, string blockName, Vector3d offset, Transaction trans)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

            // Create block reference
            BlockTableRecord blockDef = trans.GetObject(bt[blockName], OpenMode.ForRead) as BlockTableRecord;
            if (blockDef != null)
            {
                Point3d insertPoint = line.StartPoint;
                insertPoint = insertPoint.Add(offset);
                BlockReference blockRef = new BlockReference(insertPoint, blockDef.ObjectId);
                blockRef.Rotation = line.Angle;
                btr.AppendEntity(blockRef);
                trans.AddNewlyCreatedDBObject(blockRef, true);
            }
        }
        [CommandMethod("DeleteEntities")]
        public void DeleteEntities()
        {
            // Get the current document
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            // Define the path to the text file
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "deleteEntities.txt");

            // Ensure the file exists
            if (!File.Exists(filePath))
            {
                doc.Editor.WriteMessage("\nError: File not found.");
                return;
            }

            // Start a transaction
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                try
                {
                    // Open the BlockTable for read
                    BlockTable bt = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                    // Open the BlockTableRecord ModelSpace for write
                    BlockTableRecord btr = trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    // Read the file line by line
                    foreach (string line in File.ReadLines(filePath))
                    {
                        // Trim any whitespace
                        string handleStr = line.Trim();

                        // Ensure the handle string is not empty
                        if (string.IsNullOrEmpty(handleStr))
                            continue;

                        // Convert the string to an ObjectId
                        Handle handle = new Handle(Convert.ToInt64(handleStr, 16));
                        ObjectId objId = db.GetObjectId(false, handle, 0);

                        // Check if the ObjectId is valid
                        if (objId.IsValid)
                        {
                            // Open the entity for write
                            Entity entity = trans.GetObject(objId, OpenMode.ForWrite) as Entity;

                            if (entity != null)
                            {
                                // Erase the entity
                                entity.Erase();
                            }
                        }
                    }

                    // Commit the transaction
                    trans.Commit();
                    doc.Editor.WriteMessage("\nEntities deleted successfully.");
                }
                catch (System.Exception ex)
                {
                    doc.Editor.WriteMessage($"\nError: {ex.Message}");
                    trans.Abort();
                }
            }
        }
    }
}
