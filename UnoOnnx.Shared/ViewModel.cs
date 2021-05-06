using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace UnoOnnx
{
    public class ViewModel : INotifyPropertyChanged
    {
        private const string DefaultText = "Sarah lives in London and works for Acme Bank Ltd.";

        private readonly RelayCommand _goCommand;

        private string _text = DefaultText;
        private IReadOnlyCollection<EntityModel> _entities = Array.Empty<EntityModel>();

        private readonly Lazy<Prediction.IEngine> _engine = new Lazy<Prediction.IEngine>(Platform.Prediction.Engine.Factory.Create);

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            _goCommand = new RelayCommand(_ => true, _ => RunPrediction(_text));
        }

        private void RunPrediction(string text)
        {
            var results = _engine.Value.Predict(text);

            _entities = text
                .Split(" ")
                .Select((word, index) => (Word: word, Index: index))
                .GroupJoin(
                    results,
                    tuple => tuple.Index,
                    result => result.Index,
                    (tuple, results) => results
                        .Select(result => new EntityModel(tuple.Word, result.Label, result.Score))
                        .FirstOrDefault() ?? new EntityModel(tuple.Word, string.Empty, 0.0f))
                .ToArray();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Entities)));
        }

        public string Text
        {
            get { return _text; }
            set 
            {
                if (value != _text)
                {
                    _text = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
                }
            }
        }

        public IReadOnlyCollection<EntityModel> Entities
        {
            get { return _entities; }
        }

        public ICommand ChangeCommand => _goCommand;
    }
}
