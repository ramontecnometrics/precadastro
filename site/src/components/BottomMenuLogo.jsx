import React from 'react';
import logo from '../contents/img/logo-branco.svg';
import '../contents/css/logo-menu-inferior.css';

export default function BottomMenuLogo(props) {
    return (
        <img
            src={logo}
            alt="Logo"
            className={'logo-menu-inferior' + (props.className ? ' ' + props.className : '')}
            style={props.style}
        />
    );
}
