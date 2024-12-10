from app import app
from flask import request, jsonify
from gsi_data_receiver import DataReceiver
from json import load


data_receiver = DataReceiver()

@app.route('/', methods=['POST'])
def receive_data():
    '''
    This function handles POST requests
    with GSI data in JSON format. 
    It processes the data using the DataReceiver class,
    extracts relevant information into JSON files, 
    and updates the database with the latest GSI data.
    '''
    try:
        latest_data = request.get_json()
        data_receiver.get_gsi_data(latest_data)
        return jsonify(status="success"), 200
    except Exception as e:
        print(f"Error processing data: {e}")
        return jsonify(status="error", error=str(e)), 500
    
@app.route('/statistics', methods=['GET'])
def get_statistics():
    '''This function reads the statistics.json file containing 
    player game statistics and returns the data in JSON format.
    '''
    try:
        with open("statistics.json", "r") as file:
            statistics = load(file)
        return jsonify(statistics), 200
    except Exception as e:
        print(f"Error getting statistics: {e}")
        return jsonify(status="error", error=str(e)), 500

@app.route('/player_positions', methods=['GET'])
def get_positions():
    '''This function reads the statistics.json file containing 
    player position coordinates and returns the data in JSON format.
    '''
    try:
        with open("player_positions.json", "r") as file:
            statistics = load(file)
        return jsonify(statistics), 200
    except Exception as e:
        print(f"Error getting statistics: {e}")
        return jsonify(status="error", error=str(e)), 500
    
@app.route('/match_data', methods=['GET'])
def get_match():
    '''This function reads the statistics.json file containing 
    match statistics like map and round etc. and returns the data in JSON format.
    '''
    try:
        with open("match_data.json", "r") as file:
            statistics = load(file)
        return jsonify(statistics), 200
    except Exception as e:
        print(f"Error getting statistics: {e}")
        return jsonify(status="error", error=str(e)), 500