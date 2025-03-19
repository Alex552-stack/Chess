import { useDispatch, useSelector } from "react-redux";
import { AppDispatch, RootState } from "../../global-store/store";
import {
  decrement,
  increment,
  incrementByAmount,
  incrementAsync,
} from "./counterSlice";
import { useState } from "react";

const Counter = () => {
  const count = useSelector((state: RootState) => state.counter.value);
  const dispatch = useDispatch<AppDispatch>();

  const [amount, setAmount] = useState(10);

  return (
    <div>
      <h2>{count}</h2>
      <div>
        <button onClick={() => dispatch(increment())}>Increment</button>
        <input
          type="number"
          value={amount}
          onChange={(e) => setAmount(Number(e.target.value))}
        />
        <button onClick={() => dispatch(incrementByAmount(amount))}>
          Increment By{" "}
        </button>
        <button onClick={() => dispatch(incrementAsync(10))}>
          Increment Async
        </button>
        <button onClick={() => dispatch(decrement())}>Decrement</button>
      </div>
    </div>
  );
};

export default Counter;
