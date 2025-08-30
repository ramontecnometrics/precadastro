import React from 'react';
import { PatternFormat } from 'react-number-format';
import '../contents/css/cpf-input.css';

export default function CpfInput(props) {
    return (
        <PatternFormat 
            className={'form-control cpf-input' + (props.className ? (' ' + props.className) : '')}
            format='###.###.###-##'
            defaultValue={props.defaultValue}
            onValueChange={(values) => props.onChange(values.formattedValue, values.floatValue)}
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
