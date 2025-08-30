import React from 'react';
import { PatternFormat } from 'react-number-format';
import '../contents/css/time-input.css';

export default function TimeInput(props) {
    return (
        <PatternFormat
            className={'form-control time-input' + (props.className ? ' ' + props.className : '')}
            format='##:##'
            defaultValue={props.defaultValue}
            onValueChange={props.onChange}
            onBlur={props.onBlur}
            style={props.style}
            name={props.name}
            id={props.id}
            key={props.key}
            ref={props.ref}
            readOnly={props.readOnly}
        />
    );
}
