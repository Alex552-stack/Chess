import { useState, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import { Colors } from "../models/Colors";
import {Chess} from "chess.js";

export function useSignalR() {
    const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
    const [gameId, setGameId] = useState<string | null>(null);
    const [color, setColor] = useState<Colors | null>(null);
    const [game, setGame] = useState<Chess | null>(null);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5001/chesshub")
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        newConnection.start()
            .then(() => {
                console.log("Connected to SignalR hub");
                setConnection(newConnection);
            })
            .catch((err) => console.error("Connection failed:", err));

        return () => {
            newConnection.stop();
        };
    }, []);

    useEffect(() => {
        if (connection) {
            connection.on("GameStarted", (gameId, fenNotation) => {
                setGameId(gameId);
                console.log("Game Started:", gameId, "FEN:", fenNotation);
                setGame(new Chess(fenNotation));
            });

            connection.on("GetColor", (receivedColor: Colors) => {
                setColor(receivedColor);
                console.log('Assigned color:', receivedColor);
            });
            connection.on("UpdateBoard", (updatedFen: string) => {
                console.log("Update Board:", updatedFen);
                setGame(new Chess(updatedFen));
            })
        }

        return () => {
            if (connection) {
                connection.off("GameStarted");
                connection.off("GetColor");
            }
        };
    }, [connection]);

    const createGame = async () => {
        if (connection) {
            await connection.invoke("CreateGame");
            // setGameId(newGameId);
            // console.log("Game Created:", newGameId);
        }
    }

    return { connection, gameId, color, game, createGame };
}
