import React from 'react';
import './../contents/css/tab.css';

import { Tabs as BootstrapTabs, Tab as BootstrapTab } from 'react-bootstrap';

function Tabs(props) {
   return <BootstrapTabs {...props}>{props.children}</BootstrapTabs>;
}

function Tab(props) {
   return <BootstrapTab {...props}>{props.children}</BootstrapTab>;
}

export { Tabs, Tab };
