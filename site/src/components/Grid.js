import React from 'react';
import { Col as BootstrapCol, Row as BootstrapRow } from 'react-bootstrap'; 

function Col(props) {
   return <BootstrapCol {...props}>{props.children}</BootstrapCol>;
}

function Row(props) {
   return <BootstrapRow {...props}>{props.children}</BootstrapRow>;
}

export { Col, Row };