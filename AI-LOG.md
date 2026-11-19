# AI-LOG

Write down, in order, what you asked the AI. It is not a punishment: a missing or false log lowers the level D grade.

| Time (approx.) | What you asked (briefly) | What you kept | What you changed or rejected |
|---|---|---|---|
| ~ 2 min | Copilot: am cerut sa analizeze proiectul initial pentru a descoperi ce baguri legate de R1-R8 a gasit si folosind skillul de code reviw sa imi afiseze o lista unde prioritate au erorile in logica de Buissnes R1-R8 iar apoi rute, coduri HTTP, JSON camelCase, enum-uri ca text, date UTC ISO 8601 | am salvat lista pentru a putea analiza mai eficient problemele | nimic, nu a schimbat nimic la cod, doar am salvat lista pentru a putea analiza mai eficient problemele (P.S. problemele au fost depistate deja ulterior dupa refactorizare, daca pe scurt, nu a depistat toate problemele) |

|---|---|---|---|
| ~ 4 min cu tot cu analizarea codului propus | Copilot, folosind skillul creat in cadrul proiectului de la laborator am implementat; Error, ErrorType, Result<T> | am pastrat refactorizarea, trecerea de la aruncarea exceptiilor la resolver (in toate fisierele care aveau nevoie de asta), modificarea din Program.cs, stergerea a ExceptionMiddleware, BuissnesExceptions | Am respins implementarea Error.cs, ProblemDetailsExtensions(IResult), si implementarea Result, am implementat toate acestea manual de asemenea prin alt promt a fost facut Result generic (vedeti si in fisierul DECISIONS.md  punctul 5)|

|---|---|---|---|
| 1 min | Copilot, actualizare Result non-generic realizat la promtul anterior la cel generic (implementat acum) | 7 linii de cod actualizate, a eliminat result | Nimic |

|---|---|---|---|
|  | Refactorizare finala, sarcina era doar sa se asigure ca totul e schimbat corect si se afla pe sliceul sau, codul compileaza si nu sunt schimbate contractele API  | rezolvarea erorilor de compilare din Infrastructue in Repository | tentative de modificare logica |

|---|---|---|---|
|2min |Claude: generarea unui skill ce verifica implementarea sau problemele implementarii cerute de la un Punct specific din ASSIGMENT.md |Totul |Ca cerinta care vreau sa fie modificata sa o introduc eu si sa ma intrebe fisierele pe care urmeaza sa fie analizate|

|---|---|---|---|
| 4 min |  Verificare daca Punctul A din cerinta este indeplinit| Lista de probleme in implementare | Unele probleme care nu sunt relevante inca (dee exemplu testele, o sa le implementez dupa) |

|---|---|---|---|
| 3 min | Adauga time provider | implementarea initiala | am adaugat validarea pentru a respecta si R6 |

|---|---|---|---|
| 1-2 min | Schimba toti public set in private | Am pastrat toate modificarile, a schimbat doar ce trebuie | Nimic |

|---|---|---|---|
| 6 min | implementare ICommandHandler, IQueryhandler | Am pastrat a 2-a implementare implementare | Prima implementare, am imbunatatit promtul dand unul mai precis (skill generat de Claude) |

|---|---|---|---|
| 1-2 min | realizarea unui nou promt/skill pentru a verifica si implementa punctul C | Promtul pentru a testa implementarea ce urmeaza | un step 0 pentru a scanace repo exista, si ce bugs legate de R7 gaseste, apoi dupa prima iteratie au mai fost adaugate cateva buguri deja cunoscute|

|---|---|---|---|
| 1 min faza 1, 1-2 min  faza 1 nr.2 de verificare, 3 min faza de rezolvare,  | Copilot: Skillul anterior | prima data am pastrat doar lista de bug-uri, a 2-a oara am pastrat tot | Nimic |

|---|---|---|---|
|1 min |Adaptarea/creare dupa exemplul skillului de CR a unui nou skil pentru verificarea cerintelor R1-R11 |Noul skill|Nimic|

|---|---|---|---|
|2-3 min |Claude: Verificarea logicii utilizant skillul anterior (am incarcat o parte din fisierele mai importante ) |Nimic (prea costisitor), raspunsul a fost un zip |totul rejected, am actualizat specificatiile sa imi dea DOAR lista de probleme |

|---|---|---|---|
|---|Rezolvarea bug-urilor |---|---|