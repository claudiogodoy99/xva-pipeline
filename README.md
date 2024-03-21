# XVA Laboratório

Este repo tem o objetivo de documentar as provas de conceito, e estudo envolvendo XP/Microsoft, com o objetivo de criar uma pipeline para agendamento e processamento de XVA, usando [Azure Batch Account](https://learn.microsoft.com/azure/batch/).

## Arquitetura

A arquitetura deste repo é modular e todos os componentes podem ser rodados via `cli`, por exemplo.

- `xva-db-integrator`:
Este componente é um contêiner responsável por realizar operações de long polling em um banco de dados SQL local.
Ele opera com base em um cronograma cron, executando tarefas em horários específicos.
Quando determinadas condições são atendidas ou eventos ocorrem no banco de dados, ele dispara um evento que é enviado para um Azure Event Hub.

- `xva-file-event-controller`: Ao receber eventos do Event Hub, este contêiner os processa. Ele chama uma API externa, que é uma API Python publicada localmente. A API externa gera um arquivo de entrada com base nos dados do evento recebido. Após gerar o arquivo,  `xva-file-event-controller` o envia para o Azure Storage para armazenamento e uso posterior. Além disso, este componente se comunica com xva-batch-controller-api por meio de chamadas HTTP.

- `xva-batch-controller-api`: Este contêiner hospeda uma API da web que se integra a uma Conta do Azure Batch. Ele recebe solicitações e comandos de outros componentes, como xva-file-event-controller. A API interage com os serviços do Azure Batch para gerenciar e executar tarefas de processamento em lote.

- `xva-batch-gc`: Este componente é responsável por limpar arquivos no Azure Storage e gerenciar tarefas na Conta do Azure Batch. Ele verifica periodicamente arquivos expirados ou não utilizados no Azure Storage e os exclui. Também monitora e gerencia tarefas concluídas ou abandonadas na Conta do Azure Batch, garantindo uma utilização eficiente dos recursos.

![image](./images/diagram.png)

```dotnetcli
+------------------------------------------+
|              SQL On-Premises             |
|                 Banco de Dados           |
+------------------------------------------+
                    |
                    |
                    v
+------------------------------------------+
|             xva-db-integrator             |
|        (Contêiner de Long Polling)        |
+------------------------------------------+
                    |
                    |
                    v
+------------------------------------------+
|              Azure Event Hub              |
|         (Armazenamento e Eventos)         |
+------------------------------------------+
                    |
                    |
                    v
+------------------------------------------+
|        xva-file-event-controller          |
|     (Processamento de Eventos e API)      |
+------------------------------------------+
                    |
                    |
                    v
+------------------------------------------+
|              Azure Storage                |
|        (Serviço de Armazenamento)         |
+------------------------------------------+
                    |
                    |
                    v
+------------------------------------------+
|        xva-batch-controller-api           |
|           (Contêiner da API Web)          |
+------------------------------------------+
                    |
                    |
                    v
+------------------------------------------+
|            Conta do Azure Batch           |
|      (Processamento e Execução em Lote)   |
+------------------------------------------+
                    |
                    |
                    v
+------------------------------------------+
|                xva-batch-gc               |
|   (Limpeza de Arquivos e Gerenciamento)   |
+------------------------------------------+

```
