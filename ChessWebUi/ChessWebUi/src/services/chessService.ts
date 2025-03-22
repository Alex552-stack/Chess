import * as signalR from "@microsoft/signalr";

export async function createGame(connection: signalR.HubConnection | null) {
    if (connection) {
        await connection.invoke("CreateGame");
    }
}

export async function makeMove(
    source: string,
    target: string,
    piece: any,
    connection: signalR.HubConnection | null,
    gameId: string | null
) {

    if (connection && gameId) {
        await connection.invoke("MakeMoveWeb", gameId, {
            From: source,
            To: target,
            Prommotion: piece[1].toLowerCase() ?? "q",
        });
    }

    return true;
}
