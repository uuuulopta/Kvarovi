import { useState,  useMemo, useContext, useRef } from 'react';
import { Text, View,  StyleSheet } from 'react-native';
import { figmaToDeviceRatio } from './Utils';
import { SelectedChipContext, UserContext, announcementData, globals, keywordChip } from './globals';
import {  List, SegmentedButtons, Chip } from 'react-native-paper';
import { KvaroviAccordion, kvarAccordionProps } from './KvaroviAccordion';
import { useNavigation, useRoute } from '@react-navigation/native';
import { SafeAreaView } from 'react-native-safe-area-context';



const containsKw = (selectedChips: keywordChip[], text: string) => {
  for (var i = 0; i < selectedChips.length; i++) {
    if (!selectedChips[i].selected) continue;
    if (text.toLowerCase().includes(selectedChips[i].word.toLowerCase())) return true;
  }
  return false;
}

const mapAnnouncementDataToKvarAccordionProps = (data: announcementData[], instituion: string): { planned: kvarAccordionProps[], current: kvarAccordionProps[] } => {

  var planned = [];
  var current = [];
  if (!data) return { planned, current }
  for (var i = 0; i < data.length; i++) {
    if (data[i].announcementType != instituion) continue;
    var item: kvarAccordionProps = {
      date: data[i].date,
      title: data[i].title,
      titleShort: data[i].title,
      text: data[i].text,
      id: data[i].id,
      expanded: false
    }

    if (data[i].workType == "Planned") planned.push(item)
    else current.push(item)
  }
  return { planned, current }
}

export function ListingsScreen(props: { institution: string }) {




  let navigation = useNavigation();

  let route = useRoute();
  const { keywords, setKeywords } = useContext(UserContext).keywordState;
  const { announcements, setAnnouncements } = useContext(UserContext).announcementState;

  const [chipSelection, setChipSelection] = useState<keywordChip[]>(keywords ? keywords.map((k: any) => { k.selected = true; return k; }) : [])
  const [expandedId, setExpandedId] = useState<string>()
  const [segmentSelection, setSegmentSelection] = useState<"trenutni" | "planirani">("trenutni");

  let params = route.params as any
  useMemo(() => {

    if (params && params.segmentSelection) setSegmentSelection(params.segmentSelection)
  }, [params])

  let { planned, current } = mapAnnouncementDataToKvarAccordionProps(announcements, props.institution)
  const [kvarovi, setKvarovi] = useState<kvarAccordionProps[]>(current);

  //{date:"1.1.2024",titleShort:"Planirani radovi",title:"Dugacki naslov",text:lorem,id:"4",expanded:false}
  const [planirani, setPlanirani] = useState<kvarAccordionProps[]>(planned)
  function handleChange(id) {
    let arr: kvarAccordionProps[];
    if (segmentSelection == "planirani") arr = planirani
    else arr = kvarovi
    arr = arr.map((a) => {
      if(a.id == id) a.expanded = !a.expanded;
      return a;
    })
    if(segmentSelection == "planirani") setPlanirani([...planirani ])
    else setKvarovi([...kvarovi])
    if (expandedId != id) setExpandedId(id);
    else setExpandedId('')
    console.log("handlechange")


  }

  function handleChipSelect(id) {
    const newChips = chipSelection.map((c) => {
      const newC = Object.assign({}, c)
      if (newC.id == id) newC.selected = !(newC.selected)
      return newC;
    })
    setChipSelection([...newChips])

  }




  function getSelectedAccordions() {
    let arr: kvarAccordionProps[];
    if (segmentSelection == "planirani") arr = planirani
    else arr = kvarovi
    return arr && arr.length != 0 ? arr.filter((k) => containsKw(chipSelection, k.text)).map((k) =>
      <KvaroviAccordion accordionProps={k} key={k.id} selectedKeywords={chipSelection} />)
      : <Text style={{ fontFamily: "Dosis_700Bold", fontSize: 16, color: "#4E4953" }}>Trenutno nema rezultata za vaše ključne reči!</Text>


  }

  return <SafeAreaView style={{ width: "100%", height: "100%", backgroundColor: globals.background, display: "flex", flexDirection: "row", justifyContent: "center", flexWrap: "wrap" }}>
    <View style={{ display: "flex", justifyContent: "flex-start", width: figmaToDeviceRatio(311, 'x') }}>
      <Text style={{ fontFamily: "Dosis_700Bold", fontSize: 16, color: "#4E4953" }}>Kvarovi {props.institution}</Text>
    </View>
    <SegmentedButtons onValueChange={setSegmentSelection as unknown as any} value={segmentSelection}
      buttons={[{ checkedColor: globals.orange, value: "trenutni", label: "Trenutni", labelStyle: styles.segmentTexStyle, icon: segmentSelection == "trenutni" ? "alert" : "alert-outline" },
      { checkedColor: globals.blue, value: "planirani", label: "Planirani", labelStyle: styles.segmentTexStyle, icon: segmentSelection == "planirani" ? "clipboard-list" : "clipboard-list-outline" }]}
      style={styles.segmentStyle}
    />
    <View style={{ display: "flex", gap: 2, flexWrap: "wrap", flexDirection: "row", justifyContent: "center", marginTop: figmaToDeviceRatio(5, 'y') }}>
      {chipSelection ? chipSelection.map((c) => <Chip onPress={() => handleChipSelect(c.id)} selected={c.selected} key={c.id}>{c.word}</Chip>) : null}
    </View>
    <View style={{ flexBasis: "100%" }} />
    <SelectedChipContext.Provider value={chipSelection}>
      <List.AccordionGroup expandedId={expandedId} onAccordionPress={handleChange}>


        {useMemo(() => getSelectedAccordions(), [segmentSelection, chipSelection,kvarovi,planirani])}

      </List.AccordionGroup>

    </SelectedChipContext.Provider>
  </SafeAreaView>
}

const styles = StyleSheet.create({
  segmentStyle:
  {
    borderRadius: 20,
    backgroundColor: "#ffffffaa",
    width: figmaToDeviceRatio(331, 'x'),
  },
  segmentTexStyle: {
    fontSize: 17,
    fontFamily: "Dosis_600SemiBold"
  }
})




