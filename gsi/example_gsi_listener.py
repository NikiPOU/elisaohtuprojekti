from flask import Flask, request, jsonify

app = Flask(__name__)

@app.route('/', methods=['POST'])
def handle_request():
    print(request.get_json())
    return jsonify(status="success"), 200


if __name__ == '__main__':
    app.run(host='localhost', port=3000)
