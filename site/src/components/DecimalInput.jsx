import React from 'react';
import { NumericFormat } from 'react-number-format';
import '../contents/css/decimal-input.css';

export default function DecimalInput(props) {
    return (
        <NumericFormat
            className={'form-control decimal-input' + (props.className ? ' ' + props.className : '')}
            thousandSeparator='.'
            decimalSeparator=','
            defaultValue={props.defaultValue}
            onValueChange={props.onChange}           
            style={props.style}
            name={props.name}
            id={props.id}
            key={props.key}
            ref={props.ref}
            allowNegative={props.allowNegative === false ? false : true}
            decimalScale={props.decimalPlaces}
        />
    );
}
