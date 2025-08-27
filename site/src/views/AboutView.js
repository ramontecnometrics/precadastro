import React from 'react';
import { Col, Container, Row, ListGroup } from 'react-bootstrap';
import Filler from '../components/Filler';
import { IconeWhatsApp } from '../components/WhatsAppIcon';
import Text from '../components/Text';

export default function AboutView() {
   const itens = [
      { key: 'Versão', value: process.env.REACT_APP_VERSION },
      {
         key: 'Telefone de contato com suporte',
         value: (
            <div>
               (31)98881-6251 &nbsp;
               <IconeWhatsApp />
            </div>
         ),
      },
      { key: 'Email de contato com suporte', value: 'andre@tecnometrics.com.br' },
   ];

   return (
      <Container fluid>
         <Filler height={10} />
         <ListGroup>
            {itens.map((i, index) => {
               return (
                  <ListGroup.Item key={index} style={{ padding: 0 }}>
                     <Row style={{ overFlow: 'hidden', margin: 0, height: 35 }}>
                        <Col style={{ backgroundColor: '#e9ecef', minHeight: '100%', display: 'flex' }}>
                           <Text style={{ margin: 'auto 0 auto 0' }}>{i.key}</Text>
                        </Col>
                        <Col style={{ minHeight: '100%', display: 'flex' }}>
                           <Text style={{ margin: 'auto 0 auto 0' }}>{i.value}</Text>
                        </Col>
                     </Row>
                  </ListGroup.Item>
               );
            })}
         </ListGroup>
      </Container>
   );
}
