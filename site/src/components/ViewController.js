import React, { useState, useEffect } from 'react';

export default function ViewController(props) {
    const [state, setState] = useState(props.initialState || {});

    useEffect(() => {
        if (props.onStateChange) {
            props.onStateChange(state);
        }
    }, [state, props.onStateChange]);

    const updateState = (newState) => {
        setState(prevState => ({
            ...prevState,
            ...newState
        }));
    };

    const getState = () => state;

    return (
        <div className={props.className} style={props.style}>
            {props.children && React.cloneElement(props.children, {
                state,
                updateState,
                getState
            })}
        </div>
    );
}
