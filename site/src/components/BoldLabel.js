import React from 'react';
import { Form } from 'react-bootstrap';
import '../contents/css/label.css';

export default function BoldLabel(props) {
    return (
        <Form.Label
            className={'label bold-label' + (props.className ? ' ' + props.className : '')}
            style={{ fontWeight: '600', ...props.style }}
            name={props.name}
            id={props.id}
            key={props.key}
            ref={props.ref}
        >
            {props.children}
        </Form.Label>
    );
}
