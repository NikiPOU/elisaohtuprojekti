# The handover document
Since this app was our team's first Unity and VR project, we faced a significant learning curve. As our time on this project has come to an end, this document serves as a handover where we share everything we’ve learned, outline the tasks we couldn’t complete, highlight known errors, and suggest areas for improvement.

## Harware requirements
If you are using the university laptop you might run into some difficulties. CS2 does not run on the uni laptops. Unity works but might have some performance issues. We all had to use desktops to be able to properly work on this app. 

## What all the python files are for
The python files are found in the data_processing folder and have been added to OpenShift. These files have docstring documentation but the basic idea is as follows:
- **gsi_data_receiver.py**: This file is responsible for receiving data. However, this step has become redundant, so feel free to refactor or remove it.
- **gsi_processor.py**: This file processes and reorganizes the GSI data. Currently, it primarily creates a new dictionary with filtered data. An example of processing done in this step is calculating the KDR (Kill-Death Ratio), which offloads calculations from the Unity scripts. The processed game_data dictionary is then passed to the encoder and the database updater.
- **gsi_encoder.py**: This file updates various JSON files based on the type of data (e.g., player_positions, player_data, match_data).
  
The updated JSON files are hosted on OpenShift domains, where the Unity script GSIDataReceiver.cs fetches them. For example:
https://gsi-ohtuprojekti-staging.apps.ocp-test-0.k8s.it.helsinki.fi/statistics
(Note: You must be connected to the University of Helsinki's network to access this URL.) The routing is taken care of in the **routes.py** file.

## Unity
Our project uses the **2022.3.45f1** version of the Unity editor.

Never touched Unity before? Not to worry, we hadn't either. When starting with Unity, [Unity Essentials](https://learn.unity.com/pathway/unity-essentials) is a good tutorial for the basics.

There is a bug in Unity which makes the scene view appear red instead of the actual colours (at least on Linux). Here's how to fix that:
1. Open Unity hub
2. Press the 3 dots on the specific project
3. Select "Add command line arguments"
4. Write down `-force-vulkan`
5. Save

## Database
We have initialized a PostgreSQL database, which has been deployed to OpenShift. However, it is still in its early stages and is not currently being used by the app. The schema.sql file can be found in the data_processing folder. The database is updated with new GSI data through the database.updator.py file in the same folder. At this stage, we have not yet implemented a method to access the database data from Unity.
## Live broadcast

## 3D Map
Currently the only 3D map in the project is the older version of Dust 2. Some other old maps are downloadable online but they might not work depoending how much the map has changed between versions. 

We were able to get the 3D maps directly from the game but the file sizes are were too big for this app. Just decreasing quality didn't make them small enough especially since there are multiple maps. We also tried removing unnecessary objects from the maps in Blender. This isn't an efficient solution as each map has hundreds of objects and when we tried to remove them in groups we accidentally kept removing relevant stuff. Also somehow removing some objects made the file bigger. ¯\_(ツ)_/¯

Basically this means that the 3D maps would most likely have to be built from the beginning to ensure a simple enough map and small enough file sizes.

## Player movement scaling
The current scaling was done entirely by eye so it likely is not 100% accurate. We couldn't figure out an exact scaling formula.

## Statistics table
There is a unsolved issue with the statistics table colours not displaying correctly when the app is built for the glasses. The colours display correctly inside Unity's scene view. The lighting in the scene is a possible reason for this issue. 

The font on the table is not the font we wanted to use. This is however the only font that didn't break the padding on the columns. The current version of the table only works with a monospaced font (Courier in this case). If you want to use a font that is not monospaced the padding script would have to be changed. Currently the script measures word length in characters (only works if all characters have same width). Otherwise the word length would have to measured taking into account the length of each individual character (using pixels).

## Heatmap
We have started to create a heatmap. In this context, a heatmap is a visual tool used to observe where and how long players move in different locations during a CS game. We have a 2D map object in Unity, which serves as a minimap of Dust II. Additionally, we have implemented a script called "ToggleVisibility" to toggle the heatmap on and off.

There is an object called heatmapPlayer, which has a script attached named "HeatmapPlayerMovement." This script is responsible for generating the heat that appears on the heatmap.

To see the heatmap in action, you need a running CS game that provides GSI (Game State Integration) data. Follow these steps:

1. Toggle the heatmap on from the MinimapLayerMenu.
2. Select a player from the Heatmap Menu and press the player's name.
3. If you are using the XR Device Simulator, press "T" on your keyboard and then click on the player's name.
   
Once enabled, the heatmap will display that player's movement in real-time, updating based on the GSI data.

Colors
In this heatmap, we use cold colors (e.g., blue) to mark areas where the player has spent little time. Warmer colors (e.g., red) represent areas where the player has spent more time. The heatmap changes color gradually, creating a gradient effect to indicate time spent at a specific location. The longer the player stays in a location, the warmer the color becomes.

Logic
The "HeatmapPlayerMovement" script is responsible for generating the heat on the map. The logic involves creating differently colored clones of the Trace prefab, which is referred to as "TraceMaker" in the hierarchy. These clones are used to represent the varying intensities of the heatmap.

## Openshift

## Testing
We have initialized Pylint for python files.

There are a few unit tests made but they have not been updated so they are most likely not functioning. We chose to focus on other things as the unit tests were quite difficult and would have taken a lot of time. [Here](https://www.youtube.com/watch?app=desktop&v=PDYB32qAsLU&t=0s) is a simple tutorial video for unit testing in Unity.
