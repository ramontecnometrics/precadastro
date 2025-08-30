import React, { useEffect, useState } from 'react';
import { Container, Col, Row } from 'react-bootstrap';
import Button from '../components/Button';
import CheckBox from '../components/CheckBox';
import Filler from '../components/Filler';
import { showConfirm } from '../components/Messages';
import TextArea from '../components/TextArea';
import api from '../utils/Api';
import sessionManager from '../SessionManager';

export default function TermosDeUsoView() {
   const [termosDeUso, setTermosDeUso] = useState(null);
   const [iniciado, setIniciado] = useState(false);
   const [aceitouTermosDeUso, setAceitouTermosDeUso] = useState(false);

   useEffect(() => {
      api.get('/login/termosdeuso').then((result) => {
         setTermosDeUso(result.termo);
         setIniciado(true);
      });
   }, []);

   if (!iniciado) {
      return <></>;
   }

   return (
      <Container fluid>
         <Row>
            <Col>
               <Filler height={5} />
               <TextArea readOnly defaultValue={termosDeUso} />
            </Col>
         </Row>
         <br />
         <Row>
            <Col>
               <CheckBox
                  name="aceitarTermosDeUso"
                  label={'Confirmo que li e aceito os termos de uso'}
                  onChange={setAceitouTermosDeUso}
               />
            </Col>
         </Row>
         <br />
         <Row>
            <Col md={5} lg={3} xl={3}>
               <Button
                  disabled={!aceitouTermosDeUso}
                  text={'Aceitar Termos de Uso'}
                  inProgressText={'enviando...'}
                  onClickAsync={() =>
                     new Promise((resolve, reject) => {
                        showConfirm(
                           'Confirma realmente que aceita os termos de uso?',
                           () => {
                              api
                                 .post('/login/aceitartermosdeuso')
                                 .then(() => {
                                    let login = sessionManager.getLogin();
                                    login.aceitouTermosDeUso = true;
                                    sessionManager.setLogin(login);
                                    resolve();
                                    window.location = './';
                                 })
                                 .catch(reject);
                           },
                           () => reject(),
                           'Sim',
                           'Não'
                        );
                     })
                  }
               />
            </Col>
         </Row>
      </Container>
   );
}
