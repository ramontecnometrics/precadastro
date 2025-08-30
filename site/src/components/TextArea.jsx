import React from 'react';
import { Form } from 'react-bootstrap';
import '../contents/css/text-input.css';

export default function TextArea(props) {
    return (
        <Form.Control
            as="textarea"
            rows={props.rows || 3}
            className={'form-control textarea-input' + (props.className ? ' ' + props.className : '')}
            defaultValue={props.defaultValue}
            onChange={(e) => props.onChange && props.onChange(e.target.value)}
            onBlur={props.onBlur}
            style={props.style}
            name={props.name}
            id={props.id}
            key={props.key}
            ref={props.ref}
            readOnly={props.readOnly}
            placeholder={props.placeholder}
        />
    );
}
