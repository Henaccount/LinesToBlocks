Grid to Blocks sample:
- Use Plant 3D grid function to create a grid
- explode the grid with "explode" command
- copy all blocks that you want to use into this file
- load the compiled dll with netload to AutoCAD (Plant 3D)
- use this example, command: REPLACELINESWITHBLOCKS, it will ask to select the lines resulting from the grid
- this code is shaped to certain block examples that correspond with lines of certain lengths, so can only work under certain circumstances

Delete all entities sample:
- command: DELETEENTITIES
- expects a file called deleteEntities.txt in the Documents folder with AutoCAD entity handles listed, one per line
- deletes all the objects with these handles (entity handle is unique in an AutoCAD DWG file)
  
