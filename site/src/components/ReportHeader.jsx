import React from 'react';
import { Row, Col } from 'react-bootstrap';
import '../contents/css/page.css';

export default function ReportHeader(props) {
    return (
        <Row
            className={'report-header' + (props.className ? ' ' + props.className : '')}
            style={props.style}
            name={props.name}
            id={props.id}
            key={props.key}
            ref={props.ref}
        >
            <Col>
                {props.children}
            </Col>
        </Row>
    );
}
