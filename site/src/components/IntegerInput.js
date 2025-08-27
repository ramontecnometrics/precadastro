import React from 'react';
import { NumericFormat } from 'react-number-format';
import '../contents/css/integer-input.css';

export default function IntegerInput(props) {
    return (
        <NumericFormat
            className={'form-control integer-input' + (props.className ? ' ' + props.className : '')}
            thousandSeparator='.'
            defaultValue={props.defaultValue}
            onValueChange={props.onChange}
            style={props.style}
            name={props.name}
            id={props.id}
            key={props.key}
            ref={props.ref}
            allowNegative={props.allowNegative === false ? false : true}
        />
    );
}
