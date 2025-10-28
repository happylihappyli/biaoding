extends Node

var _server: TCPServer = TCPServer.new()
var _client: StreamPeerTCP = null
var PORT = 9090

func start_server():
    if _server.listen(PORT) == OK:
        print("Server started on port ", PORT)
    else:
        print("Failed to start server")

func _ready():
    start_server()

func _process(delta: float) -> void:
    if _server.is_connection_available():
        _client = _server.take_connection()
        print("Client connected:", _client.get_connected_host())

    if _client != null and _client.get_status() == StreamPeerTCP.STATUS_CONNECTED:
        var available_bytes = _client.get_available_bytes()
        if available_bytes > 0:
            var data = _client.get_utf8_string(available_bytes)
            if data:
                handle_command(data)

func handle_command(command: String):
    print(command)

