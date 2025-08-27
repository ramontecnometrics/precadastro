import React, { useState, useEffect, Fragment, useRef, useMemo } from 'react';
import { Container, Row, Col, Form } from 'react-bootstrap';
import { LayoutParams } from '../../../config/LayoutParams';
import Label from '../../Label';
import Button from '../../Button';
import TextInput from '../../TextInput';
import PasswordInput from '../../PasswordInput';
import CheckBox from '../../CheckBox';
import Filler from '../../Filler';
import './../../../contents/css/login-logo.css';
import { Link } from 'react-router-dom';
import LoginController from './LoginController';
import { showError } from '../../Messages';
import ReCAPTCHA from 'react-google-recaptcha';
import Text from '../../Text';

export default function LoginView(props) {
   const [lembrarLogin, setLembrarLogin] = useState(localStorage.getItem('lembrarLogin') === 'true');
   const [nomeDeUsuario, setNomeDeUsuario] = useState(() => {
      const nome = localStorage.getItem('nomeDeUsuario');
      return nome === 'null' ? null : nome;
   });
   const [senha, setSenha] = useState(null);
   const [recaptcha, setRecaptcha] = useState(null);

   const recaptchaRef = useRef(null);

   const controller = useMemo(() => new LoginController(), []);

   useEffect(() => {
      window.onhashchange = () => {
         if (window.location.hash !== '#/' && window.location.hash !== '#/adm/recoverpassword') {
            window.location.reload();
         }
      };
   }, []);

   const getDescricaoDoTipoDeAcesso = () => {
      if (props.tipoDeAcesso === 'ADM') {
         return 'Administrador';
      }
      return null;
   };

   const formLoginTecnometrics = () => (
      <Fragment>
         <div
            style={{
               fontSize: 16,
               fontWeight: 'bold',
               textAlign: 'center',
               marginTop: -30,
               fontStyle: 'italic',
            }}
         >
            <Text>Tipo de acesso:&nbsp;</Text>
            <Text>{getDescricaoDoTipoDeAcesso()}</Text>
            <br />
            <Filler height={6} />
         </div>
         <Form
            onSubmit={(event) => {
               console.log('onSubmit');
               event.preventDefault();
               if (lembrarLogin) {
                  localStorage.setItem('lembrarLogin', true);
                  localStorage.setItem('nomeDeUsuarioAdministrador', nomeDeUsuario);
               } else {
                  localStorage.setItem('lembrarLogin', null);
                  localStorage.setItem('nomeDeUsuarioAdministrador', null);
               }
               controller.efetuarLogin(nomeDeUsuario, senha, recaptcha).catch((e) => {
                  if (e) {
                     showError(e.toString());
                  }
               });
            }}
            action='#'
            id='formLogin'
            className='justify-content-md-center'
         >
            <Filler height={20} />

            <Form.Group>
               <div style={{ display: 'flex' }}>
                  <div style={{ display: 'flex', width: '100%' }}>
                     <Label>Nome de usuário</Label>
                  </div>
                  <div style={{ display: 'flex', minWidth: 'fit-content' }}>
                     <CheckBox
                        className='fs-12'
                        label='Lembrar nome de usuário'
                        defaultChecked={lembrarLogin}
                        onChange={(checked) => setLembrarLogin(checked)}
                     />
                  </div>
               </div>
               <TextInput
                  defaultValue={nomeDeUsuario}
                  name='nomeDeUsuario'
                  onChange={(value) => setNomeDeUsuario(value)}
               />
            </Form.Group>

            <Filler height={20} />

            <Form.Group>
               <div style={{ display: 'flex' }}>
                  <div style={{ display: 'flex', width: '100%' }}>
                     <Label>Senha</Label>
                  </div>
                  <div style={{ display: 'flex', color: LayoutParams.colors.corSecundaria }}>
                     <Link to={`/adm/recoverpassword/${props.tipoDeAcesso.toLowerCase()}`} tabIndex={-1}>
                        <Label style={{ whiteSpace: 'nowrap', cursor: 'pointer' }}>
                           <Text>Esqueceu a senha?</Text>
                        </Label>
                     </Link>
                  </div>
               </div>
               <PasswordInput
                  name='senha'
                  type='password'
                  defaultValue={senha || ''}
                  onChange={(value) => setSenha(value)}
               />
            </Form.Group>

            <Filler height={20} />

            <div style={{ minHeight: 78, maxWidth: 304, marginLeft: 'auto', marginRight: 'auto' }}>
               <ReCAPTCHA
                  sitekey='6LclosMgAAAAAMpNQ7pbzKmyl2v_2UGY1exIzHvH'
                  ref={recaptchaRef}
                  onChange={(response) => setRecaptcha(response)}
                  className='div-recaptcha'
                  hl='pt-BR'
               />
            </div>

            <br />

            <Button
               type='submit'
               text='Entrar'
               inProgressText='entrando...'
               style={{
                  cursor: recaptcha ? 'pointer' : 'not-allowed !important',
                  width: '100%',
               }}               
            />
         </Form>
      </Fragment>
   );

   return (
      <Container
         fluid
         id='container'
         style={{
            fontSize: 15,
            height: '100%',
            display: 'flex',
            position: 'fixed',
            backgroundColor: LayoutParams.colors.corDoTemaPrincipal,
            overflowX: 'auto',
            flexDirection: 'column',
         }}
      >
         <Row className='justify-content-md-center'>
            <Col
               xs
               lg='3'
               style={{
                  minWidth: 330,
                  maxWidth: 330,
                  borderRadius: 10,
                  color: 'white',
                  paddingTop: 60,
                  marginLeft: 'auto',
                  marginRight: 'auto',
               }}
            >
               <br />
               {props.tipoDeAcesso === 'ADM' ? formLoginTecnometrics() : null}
               <br />
            </Col>
         </Row>
      </Container>
   );
}
