import { Chessboard } from "react-chessboard";
import {Colors} from "../models/Colors.ts";

interface ChessboardComponentProps {
    position: string;
    handleMove: (sourceSquare: string, targetSquare: string, piece: any) => Promise<boolean>;
    color : Colors | null;
}

export default function ChessboardComponent({ position, handleMove, color }: ChessboardComponentProps) {
    return (
        <div style={{ width: "500px", height: "500px" }}>
            <Chessboard
                position={position}
                onPieceDrop={(sourceSquare, targetSquare, piece) => {
                    handleMove(sourceSquare, targetSquare, piece); // Fire and forget
                    return true; // Always accept move immediately
                }}
                animationDuration={0}
                boardOrientation = {color == Colors.Black ? "black" : "white" }
            />
        </div>
    );
}
