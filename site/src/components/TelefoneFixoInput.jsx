import React from 'react';
import { PatternFormat } from 'react-number-format';
import '../contents/css/telefone-fixo-input.css';

export default function TelefoneFixoInput(props) {
    return (
        <PatternFormat
            className={'form-control telefone-fixo-input' + (props.className ? ' ' + props.className : '')}
            format='(##) ####-####'
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
