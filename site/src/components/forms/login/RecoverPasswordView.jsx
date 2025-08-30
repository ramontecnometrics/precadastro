import React, { useState, useEffect, Fragment, useRef, useMemo } from 'react';
import { Container, Row, Col, Form } from 'react-bootstrap';
import { LayoutParams } from '../../../config/LayoutParams';
import Label from '../../../components/Label';
import Button from '../../../components/Button';
import TextInput from '../../../components/TextInput';
import { Link } from 'react-router-dom';
import PasswordInput from '../../../components/PasswordInput';
import RecoverPasswordController from './RecoverPasswordController';
import ReCAPTCHA from 'react-google-recaptcha';
import LogoLogin from './LoginLogo';

export default function RecoverPasswordView(props) {
   const [codigoDeSeguranca, setCodigoDeSeguranca] = useState('');
   const [novaSenha, setNovaSenha] = useState('');
   const [confirmacaoDaSenha, setConfirmacaoDaSenha] = useState('');
   const [codigoDeSegurancaEnviado, setCodigoDeSegurancaEnviado] = useState(false);
   const [mostrarRecaptcha, setMostrarRecaptcha] = useState(false);
   const [nomeDeUsuario, setNomeDeUsuario] = useState('');
   const [recaptcha, setRecaptcha] = useState(null);

   const recaptchaRef = useRef(null);

   // Instancia o controller com hooks
   const controller = useMemo(
      () =>
         new RecoverPasswordController({
            props,
            get state() {
               return {
                  codigoDeSeguranca,
                  novaSenha,
                  confirmacaoDaSenha,
                  codigoDeSegurancaEnviado,
                  mostrarRecaptcha,
                  nomeDeUsuario,
                  recaptcha,
               };
            },
            setCodigoDeSeguranca,
            setNovaSenha,
            setConfirmacaoDaSenha,
            setCodigoDeSegurancaEnviado,
            setMostrarRecaptcha,
            setNomeDeUsuario,
            setRecaptcha,
         }),
      [
         props,
         codigoDeSeguranca,
         novaSenha,
         confirmacaoDaSenha,
         codigoDeSegurancaEnviado,
         mostrarRecaptcha,
         nomeDeUsuario,
         recaptcha,
      ]
   );

   useEffect(() => {
      const timer = setTimeout(() => {
         setMostrarRecaptcha(true);
      }, 1000);
      return () => clearTimeout(timer);
   }, []);

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
               <LogoLogin />
               <div className='justify-content-md-center'>
                  <Fragment>
                     {!codigoDeSegurancaEnviado && (
                        <div>
                           <Form.Group>
                              <Label>{'Usuário'}</Label>
                              <TextInput type='text' onChange={(e) => setNomeDeUsuario(e.target.value)} />
                              <br />
                              <div style={{ minHeight: 78 }}>
                                 {mostrarRecaptcha && (
                                    <ReCAPTCHA
                                       sitekey='6LclosMgAAAAAMpNQ7pbzKmyl2v_2UGY1exIzHvH'
                                       ref={recaptchaRef}
                                       onChange={(response) => setRecaptcha(response)}
                                       className='div-recaptcha'
                                       hl={'pt-BR'}
                                    />
                                 )}
                              </div>
                              <br />

                              <Button
                                 text={'Enviar código de segurança'}
                                 inProgressText={'enviando...'}
                                 onClickAsync={controller.enviarCodigoDeSeguranca}
                              />
                           </Form.Group>
                        </div>
                     )}

                     {codigoDeSegurancaEnviado && (
                        <Fragment>
                           <Form.Group>
                              <Label>{'Código recebido por email'}</Label>
                              <TextInput type='text' onChange={(e) => setCodigoDeSeguranca(e.target.value)} />
                           </Form.Group>
                           <Form.Group>
                              <Label>{'Nova senha'}</Label>
                              <PasswordInput type='password' onChange={(e) => setNovaSenha(e.target.value)} />
                           </Form.Group>
                           <Form.Group>
                              <Label>{'Confirmação da senha'}</Label>
                              <PasswordInput type='password' onChange={(e) => setConfirmacaoDaSenha(e.target.value)} />
                              <br />
                              <Button
                                 text={'Recuperar senha'}
                                 inProgressText={'processando...'}
                                 onClickAsync={controller.recuperarSenha}
                              />
                           </Form.Group>
                        </Fragment>
                     )}

                     <Link to={'/adm'} tabIndex={-1}>
                        <Button text={'Voltar para login'} />
                     </Link>
                  </Fragment>
               </div>
            </Col>
         </Row>
      </Container>
   );
}
