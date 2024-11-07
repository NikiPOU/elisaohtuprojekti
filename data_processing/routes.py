from app import app, data_receiver
from flask import request, jsonify

database_initialized = False

@app.route('/', methods=['POST'])
def receive_data():
    try:
        latest_data = request.get_json()
        data_receiver.get_gsi_data(latest_data)
        return jsonify(status="success"), 200
    except Exception as e:
        print(f"Error processing data: {e}")
        return jsonify(status="error", error=str(e)), 500