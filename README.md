# Applicazione XR Museo Egizio
## Componenti del magazzino
La maggior parte della logica dell'applicazione si trova nello script AppManager. Con la funzione GetAllShelves si recuperano tutti i componenti della warehouse presente nella scena e per ognuno di essi la funzione SetInitialTransform recupera la posizione salvata e gliel'assegna. Il salvataggio della nuova posizione è invece gestito da SaveNewPositions, che per lo scaffale selezionato ed eventuali elementi sottostanti nella gerarchia chiama la funzione SaveTransformObject, in cui si procede al salvataggio vero e proprio.
## Reperti
Per quanto riguarda i reperti questi vengono recuperati dalla scena con la funzione GetAllArtifacts, la quale chiama poi SetArtifactsShelf, in cui si recupera l'eventuale scaffale in cui il reperto si trova.
## Gestione liste
Lo script VirtualizedScrollRectListTester serve per la gestione delle varie liste presenti nell'applicazione, ad esempio quella dei reperti (con le dovute modifiche quando si efffettua una ricerca), quella degli scaffali quando si vuole depositare un reperto etc.
## Dettatura
Lo script DictationManager gestisce la dettatura per la ricerca vocale dei reperti.
## Reset del simulatore
Lo script XRRigReset serve a spegnere e riaccendere XR Rig allo start per ovviare al problema del simulatore di MRTK (non funziona sempre, a volte continua a dare errore).
