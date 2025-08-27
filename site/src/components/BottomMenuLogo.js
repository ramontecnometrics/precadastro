import React from 'react';
import '../contents/css/logo-menu-inferior.css';

export default function BottomMenuLogo(props) {
    return (
        <img
            src={require('../contents/img/logo.svg')}
            alt="Logo"
            className={'logo-menu-inferior' + (props.className ? ' ' + props.className : '')}
            style={props.style}
        />
    );
}
