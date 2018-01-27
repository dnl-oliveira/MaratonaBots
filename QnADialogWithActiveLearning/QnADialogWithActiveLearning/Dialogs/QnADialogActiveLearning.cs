using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;


namespace QnADialogWithActiveLearning.Dialogs
{
    [Serializable]   
    public class QnADialogActiveLearning : QnAMakerDialog
    {
        public QnADialogActiveLearning() : base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnASubscriptionKey"], ConfigurationManager.AppSettings["QnABaseId"], "Não encontrei sua resposta", 0.5, 1)))
        {

        }

        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            var primeiraResposta = result.Answers[0].Answer;
            Activity resposta = ((Activity)context.Activity).CreateReply();

            var dadosResposta = primeiraResposta.Split('|');

            if(dadosResposta.Length == 1)
            {
                await context.PostAsync(primeiraResposta);
                return;
            }

            var titulo = dadosResposta[0];
            var descricao = dadosResposta[1];
            var cargaHoraria = dadosResposta[2];
            var preco = dadosResposta[3];
            var urlImagem = dadosResposta[4];
            var link = dadosResposta[5];

            HeroCard card = new HeroCard
            {
                Title = titulo,
                Subtitle = descricao,
                Text = cargaHoraria + " - " + preco
            };
            card.Buttons = new List<CardAction>
            {
                new CardAction(ActionTypes.OpenUrl, "Compre Agora por apenas" + preco, value:link)
            };

            card.Images = new List<CardImage>
            {
                new CardImage(url: urlImagem)
            };

            resposta.Attachments.Add(card.ToAttachment());
            await context.PostAsync(resposta);

        }
    }    
}