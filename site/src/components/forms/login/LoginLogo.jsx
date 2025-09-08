import React from 'react';
import  '../../../contents/css/login-logo.css';
import logo from '../../../contents/img/logo-branco.svg';

export default function LoginLogo(props) {
    return (
        <img
            src={logo}
            alt="Logo"
            className={'login-logo' + (props.className ? ' ' + props.className : '')}
            style={props.style}
        />
    );
}
