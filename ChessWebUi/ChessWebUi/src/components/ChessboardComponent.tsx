import { Chessboard } from "react-chessboard";

interface ChessboardComponentProps {
    position: string;
    handleMove: (sourceSquare: string, targetSquare: string, piece: any) => Promise<boolean>;
}

export default function ChessboardComponent({ position, handleMove }: ChessboardComponentProps) {
    return (
        <div style={{ width: "500px", height: "500px" }}>
            <Chessboard
                position={position}
                onPieceDrop={(sourceSquare, targetSquare, piece) => {
                    handleMove(sourceSquare, targetSquare, piece); // Fire and forget
                    return true; // Always accept move immediately
                }}
                animationDuration={0}
            />
        </div>
    );
}
