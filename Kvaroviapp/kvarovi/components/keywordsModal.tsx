import { createRef, useCallback, useEffect, useMemo, useRef, useState } from "react"
import { View,StyleSheet,Modal, Animated, KeyboardAvoidingView} from "react-native";
import {  Chip, Text,  useTheme,Button, PaperProvider } from "react-native-paper";
import { FAB, Portal } from 'react-native-paper';
import { figmaToDeviceRatio } from "./Utils";
import { transform } from "typescript";
import BottomSheet, { BottomSheetModal, BottomSheetModalProvider, BottomSheetTextInput } from "@gorhom/bottom-sheet";
import { GestureHandlerRootView } from "react-native-gesture-handler";
import {globals,keyword} from './globals';
import { useNavigation } from "@react-navigation/native";
import { useSafeAreaFrame, useSafeAreaInsets } from "react-native-safe-area-context";
import { Dosis_700Bold } from "@expo-google-fonts/dosis";
import  PaperBottomSheetTextInput  from "./PaperBottomSheetInput"

export function KeywordsFabModal( props: {
  keywords: keyword[],
  wrapperStyle:any,
  FABstyle: object,
  color: string,
  onModalChange: () => void,

}) {

  const theme = useTheme();
  const navigation = useNavigation();
  const [modalVisible,setModalVisible] = useState<boolean>(false);
  const slideAnim = useRef(new Animated.Value(0))
  const onDismiss = () => {
    setModalVisible(false)
    slideAnim.current = new Animated.Value(0);
  }

  const onOpen = () => {
    setModalVisible(true)

  }

  const insets = useSafeAreaInsets();
  const bottomSheetModalRef = useRef<BottomSheetModal>(null);
  const snapPoints = useMemo(() => ['25%', '50%','75%','85%'], []);
  const handlePresentModalPress = useCallback(() => {
    props.onModalChange();
    bottomSheetModalRef.current?.present();

  }, [modalVisible]);
  const handleSheetChanges = useCallback((index: number) => {
    if(index == -1) props.onModalChange();
    console.log('handleSheetChanges', index);
  }, [modalVisible]);

  const [keywords,setKeywords] = useState<keyword[]>(props.keywords);
  const [kwCounter,setKwCounter] = useState<number>(1);
  const keywordInput = useRef<any>();
  const onKwAdd  = () => {
    setKeywords([...keywords,{word: keywordInput.current,id:"N" + kwCounter}] )
    setKwCounter(kwCounter + 1);
    keywordInput.current = ""
  }
  return  <View style={[ props.wrapperStyle ]}>
    <FAB onPress={handlePresentModalPress} style={props.FABstyle}  mode="elevated"  color={ props.color } icon={"pencil"}/>
    <BottomSheetModal
      keyboardBehavior="fillParent"
      keyboardBlurBehavior="restore"
      ref={bottomSheetModalRef}
      index={1}
      snapPoints={snapPoints}
      onChange={handleSheetChanges}
      handleStyle={{backgroundColor:theme.colors.onPrimaryContainer}}
      handleIndicatorStyle={{backgroundColor:"white"}}
    >
      <View style={{flexDirection:"row",flexWrap:"wrap",gap:4,justifyContent: "center"}}>  
        <Text style={{fontFamily:"Dosis_700Bold",fontSize: 28,color: theme.colors.onBackground}}>Unos ključnih reči</Text>
        <View style={{flexBasis: "100%"}}/>
        {keywords.map((k) => <Chip onClose={() => setKeywords(keywords.filter(kw => kw.id != k.id))}  closeIcon="close" key={k.id}>{k.word}</Chip>)}
        <View style={{flexBasis: "100%"}}/>
        <PaperBottomSheetTextInput
          label="Ulica / ključna reč"
          onSubmitEditing={onKwAdd}
          onChangeText={(t) => keywordInput.current = t}
          placeholder="Na primer: Vojvode stepe 12"
          style={{marginTop:figmaToDeviceRatio(20,'y'),flexGrow:0.9}}/>
        <View style={{flexBasis: "100%"}}/>
        <View style={{height: "50%",width:"100%",alignItems: "center",justifyContent:"center", flexDirection:"row-reverse"}}>
          <Button mode="contained" onPress={onKwAdd} style={{flexGrow:0.25}}>Unesi</Button>
        </View>
      </View>
    </BottomSheetModal>
  </View>
}

const styles = StyleSheet.create({
  modalMain:{
    display:'flex',
    flexDirection: 'row-reverse',
    alignItems: "flex-end",
    marginTop: 100,

  },
  modalView: {
    width: figmaToDeviceRatio(355,'x'),
    height: figmaToDeviceRatio(200,'y'),
    backgroundColor: "white",
    borderRadius: 20,
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    flexWrap: "wrap",
    flexDirection: "row",
    padding: figmaToDeviceRatio(6,'y'),
    marginBottom: figmaToDeviceRatio(9,'y')
  },
  contentContainer: {
    flex: 1,
    alignItems: 'center',
    borderTopLeftRadius: 10,
    borderTopRightRadius: 10,
    borderLeftWidth: 0.2,
    borderRightWidth: 0.2,
    height:   figmaToDeviceRatio(56,'y'),
    paddingVertical: 5,
    overflow: 'hidden',
  },

})
