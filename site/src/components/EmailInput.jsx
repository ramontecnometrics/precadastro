import React from 'react';
import { Form } from 'react-bootstrap';
import '../contents/css/text-input.css';

export default function EmailInput(props) {
    return (
        <Form.Control
            type='email'
            className={'form-control email-input text-input' + (props.className ? ' ' + props.className : '')}
            defaultValue={props.defaultValue}
            onChange={(e) => props.onChange(e.target.value)}
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
