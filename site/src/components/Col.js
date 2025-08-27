import React from 'react';
import { Col as BootstrapCol } from 'react-bootstrap';

export default function Col(props) {
   return <BootstrapCol {...props}>{props.children}</BootstrapCol>;
}
