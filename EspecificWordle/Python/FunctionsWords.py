import json
import nltk

from PyDictionary import PyDictionary
from googletrans import Translator
from pyrae import dle
from nltk.corpus import wordnet as wn

nltk.download('wordnet')
nltk.download('omw-1.4')

dictionary = PyDictionary()
translator = Translator()

def translateEn(text):
    return translator.translate(text, src='es', dest='en').text

def translateEs(text):
    return translator.translate(text, src='en', dest='es').text

def wordDefinition(word):
    syn = wn.synsets(word)
    if isinstance(syn, list) and syn:
        return syn[0].definition()
    else:
        return str()

def wordDefinitionRae(word):
    return dle.search_by_word(word=word)

def wordSynonyms(word):
    synonyms = []

    try:
        synsets = wn.synsets(word)

        if not synsets:
            return synonyms

        for syn in synsets:
            lemmas = syn.lemmas()
            for lm in lemmas:
                synonyms.append(translator.translate(lm.name(), dest='es').text)
    except Exception as e:
        raise Exception(e)

    return synonyms

def wordAntonyms(word):
    antonyms = []
    
    try:
        synsets = wn.synsets(word)
    
        if not synsets:
            return antonyms

        for syn in synsets:
            for lm in syn.lemmas():
                for ant in lm.antonyms():
                    antonym_name = ant.name()
                    if antonym_name:
                        antonyms.append(translator.translate(antonym_name, src='en', dest='es').text)
    except Exception as e:
        raise Exception(e)

    return antonyms

def wordUseExample(word):
    synsets = wn.synsets(word)
    if synsets:
        examples = synsets[0].examples()
        if examples:
            return translator.translate(examples[0], src='en', dest='es').text
        else:
            return ""
