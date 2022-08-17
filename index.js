const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/messenger-hub", {
        // by default SignalR uses a workflow associated with Access-Control-Allow-Credentials
        // reference: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Access-Control-Allow-Credentials
        // we use gateway, so just disable it
        withCredentials: false,
        accessTokenFactory: () => "put token here",
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    await connection.start();
    console.log("SignalR Connected.");
};
// You can make a reconnecting loop
// And a "onClose" callback to try reconnecting.

connection.on("ReceiveMessage", message => {
    console.log("Message from server: ", message);
});

connection.on("ReceiveExport", message => {
    console.log("Export Message from server: ", message);
});

// Start the connection.
start();

function sendMessage() {
    connection.invoke("SendMessage", "Hello");
}