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

## Database
We have initialized a PostgreSQL database, which has been deployed to OpenShift. However, it is still in its early stages and is not currently being used by the app. The schema.sql file can be found in the data_processing folder. The database is updated with new GSI data through the database.updator.py file in the same folder. At this stage, we have not yet implemented a method to access the database data from Unity.
## Live broadcast

## Heatmap

## Openshift

## Testing
