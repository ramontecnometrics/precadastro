import React from 'react';
import { Card } from 'react-bootstrap';
import '../contents/css/panel.css';

export default function Panel(props) {
    return (
        <Card
            className={'panel' + (props.className ? ' ' + props.className : '')}
            style={props.style}
            name={props.name}
            id={props.id}
            key={props.key}
            ref={props.ref}
        >
            {props.children}
        </Card>
    );
}
