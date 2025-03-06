using EntwineLlm.Converters;
using EntwineLlm.Enums;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EntwineLlm.Models
{
    public class GeneralOptions : DialogPage
    {
        [Category("Configuration")]
        [DisplayName("Large Language Model Base Url")]
        [Description("Sets the base URL for local LLM")]
        public string LlmUrl
        {
            get
            {
                return EntwineLlmPackage.LlmClient?.GetBaseUrl() ?? "http://localhost:1234/v1";
            }
            set
            {
                EntwineLlmPackage.LlmClient?.SetBaseUrl(value);
            }
        }

        [Category("Configuration")]
        [DisplayName("Requests timeout")]
        [Description("Sets timeout for HTTP requests")]

        public TimeSpan LlmRequestTimeOut
        {
            get
            {
                return EntwineLlmPackage.LlmClient?.GetTimeOut() ?? new TimeSpan(0, 10, 0);
            }
            set
            {
                EntwineLlmPackage.LlmClient?.SetTimeOut(value);
            }
        }

        [Category("Configuration")]
        [DisplayName("LLM response language")]
        [Description("Set the language in which the LLM must answer")]
        [TypeConverter(typeof(LanguageConverter))]
        public string Language { get; set; } = "English";

        [Category("Configuration")]
        [DisplayName("Select LLM agent model")]
        [Description("모델을 사용할 서비스 제공자를 선택합니다.")]

        public ClientAgentType ModelType { get; set; } = ClientAgentType.LmStudio;
    }


    public class ModelsOptions : DialogPage
    {
        [Category("Models")]
        [DisplayName("Refactor queries")]
        [Description("Sets the model to be used when querying LLM for refactor")]
        [TypeConverter(typeof(LlmModelConverter))]
        public string LlmRefactor { get; set; } = "llama3.2";

        [Category("Models")]
        [DisplayName("Unit tests generation")]
        [Description("Sets the model to be used when querying LLM for unit tests generation")]
        [TypeConverter(typeof(LlmModelConverter))]
        public string LlmUnitTests { get; set; } = "llama3.2";

        [Category("Models")]
        [DisplayName("Documentation generation")]
        [Description("Sets the model to be used when querying LLM for code documentation generation")]
        [TypeConverter(typeof(LlmModelConverter))]
        public string LlmDocumentation { get; set; } = "llama3.2";

        [Category("Models")]
        [DisplayName("Code review query")]
        [Description("Sets the model to be used when querying LLM for code review")]
        [TypeConverter(typeof(LlmModelConverter))]
        public string LlmReview { get; set; } = "llama3.2";

        [Category("Models")]
        [DisplayName("Follow-up query")]
        [Description("Sets the model to be used when querying LLM for follow-up prompts")]
        [TypeConverter(typeof(LlmModelConverter))]
        public string LlmFollowUp { get; set; } = "llama3.2";
    }
}
