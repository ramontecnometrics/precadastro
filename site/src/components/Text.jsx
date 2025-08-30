import React from 'react';

export default function Text(props) {
    return (
        <span
            className={props.className}
            style={props.style}
        >
            {props.children}
        </span>
    );
}
