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

Note: all code was generated by GPT-4o with minor corrections, following these two prompts:

Hi, I want to create C# code for AutoCAD. The script starts asking for lines to be selected. There are vertical and horizontal lines, the horizontal lines can be of different length. 
Horizonal lines can be oriented in x and y direction.
The vertical lines should get replaced with block instances of a block named "TM[1]Staander MP 200" (line length is 2000mm, position offset of 106mm in positive z direction needs to be applied), 
the horicontal lines have to be replaced by the blocks named: "TM[1]Ligger 257" (line length is 2572mm), 
"TM[1]Ligger 157" (line length is 1572mm) and "TM[1]Ligger 109" (line length is 1088mm). Of course the blocks need to be aligned so they are in line with the lines that are replaced.
The blocks have their base point in the minimum location by coordinates.

Hi, please write C# code for AutoCAD, this is what the tool shall do: read from a text file which resides in the Documents folder and is named "deleteEntities.txt". 
Read line by line all the entity handles that are listed and delete from the current drawing the object which can be identified by this entity handle.
  
