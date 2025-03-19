import { useState, useEffect } from "react";
import { Chess } from "chess.js";
import { Chessboard } from "react-chessboard";
import * as signalR from "@microsoft/signalr";

export default function GamePage() {
    const [game, setGame] = useState(new Chess());
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [gameId, setGameId] = useState<string | null>(null);

    // Initialize SignalR Connection
    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5001/chesshub") // Change this if needed
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        newConnection
            .start()
            .then(() => {
                console.log("Connected to SignalR hub");
                setConnection(newConnection);
            })
            .catch((err) => console.error("Connection failed: ", err));

        return () => {
            newConnection.stop();
        };
    }, []);

    // Function to create a game
    const createGame = async () => {
        if (connection) {
            const newGameId = await connection.invoke<string>("CreateGame");
            setGameId(newGameId);
            console.log("Game Created:", newGameId);
        }
    };

    // Function to send a move to the server
    const handleMove = async (source: string, target: string, piece: any) => {
        // @ts-ignore
        const gameCopy = new Chess(game.fen());
        const move = gameCopy.move({
            from: source,
            to: target,
            promotion: piece[1].toLowerCase() ?? "q"
        });

        if (move && connection && gameId) {
            await connection.invoke("MakeMove", gameId, move);
        }

        setGame(gameCopy);
        return move !== null;
    };

    // Listen for board updates from the server
    useEffect(() => {
        if (connection) {
            connection.on("UpdateBoard", (updatedFen: string) => {
                console.log("Board updated:", updatedFen);
                setGame(new Chess(updatedFen));
            });
        }
    }, [connection]);

    return (
        <div>
            <button onClick={createGame}>Create Game</button>
            <div style={{ width: "500px", height: "500px" }}>
                <Chessboard
                    position={game.fen()}
                    onPieceDrop={(sourceSquare, targetSquare, piece) =>
                        handleMove(sourceSquare, targetSquare, piece)
                    }
                    animationDuration={0}
                />
            </div>
        </div>
    );
}
