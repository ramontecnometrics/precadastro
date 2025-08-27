import React from 'react';
import { Row as BootstrapRow } from 'react-bootstrap';

export default function Row(props) {
   return <BootstrapRow {...props}>{props.children}</BootstrapRow>;
}
