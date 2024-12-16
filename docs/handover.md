# The handover document
Since this app was our team's first Unity and VR project, we faced a significant learning curve. As our time on this project has come to an end, this document serves as a handover where we share everything we’ve learned, outline the tasks we couldn’t complete, highlight known errors, and suggest areas for improvement.

## What all the python files are for
The python files are found in the data_processing folder and have been added to OpenShift. These files have docstring documentation but the basic idea is as follows:
- **gsi_data_receiver.py**: This file is responsible for receiving data. However, this step has become redundant, so feel free to refactor or remove it.
- **gsi_processor.py**: This file processes and reorganizes the GSI data. Currently, it primarily creates a new dictionary with filtered data. An example of processing done in this step is calculating the KDR (Kill-Death Ratio), which offloads calculations from the Unity scripts. The processed game_data dictionary is then passed to the encoder and the database updater.
- **gsi_encoder.py**: This file updates various JSON files based on the type of data (e.g., player_positions, player_data, match_data).
  
The updated JSON files are hosted on OpenShift domains, where the Unity script GSIDataReceiver.cs fetches them. For example:
https://gsi-ohtuprojekti-staging.apps.ocp-test-0.k8s.it.helsinki.fi/statistics
(Note: You must be connected to the University of Helsinki's network to access this URL.) The routing is taken care of in the **routes.py** file.

## Unity
When starting with Unity, [Unity Essentials](https://learn.unity.com/pathway/unity-essentials) is a good tutorial for the basics.

There is a bug in Unity which makes the scene view appear red instead of the actual colours (at least on Linux). Here's how to fix that:
1. Open Unity hu
2. Press the 3 dots on the specific project
3. Select "Add command line arguments"
4. Write down `-force-vulkan`
5. Save

## Database
We have initialized a PostgreSQL database, which has been deployed to OpenShift. However, it is still in its early stages and is not currently being used by the app. The schema.sql file can be found in the data_processing folder. The database is updated with new GSI data through the database.updator.py file in the same folder. At this stage, we have not yet implemented a method to access the database data from Unity.
## Live broadcast

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
