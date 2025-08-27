import React from 'react';
import { Form } from 'react-bootstrap';
import '../contents/css/radio-input.css';

export default function RadioInput(props) {
    const handleChange = (e) => {
        if (props.onChange) {
            props.onChange(e);
        }
    };

    return (
        <div className={'radio-input-container' + (props.className ? ' ' + props.className : '')}>
            {props.options && props.options.map((option, index) => (
                <Form.Check
                    key={index}
                    type="radio"
                    id={`${props.name}-${index}`}
                    name={props.name}
                    defaultValue={option.value}
                    label={option.label}
                    checked={props.value === option.value}
                    onChange={handleChange}
                    disabled={props.disabled}
                    style={props.style}
                    className={'radio-input' + (props.className ? ' ' + props.className : '')}
                />
            ))}
        </div>
    );
}

